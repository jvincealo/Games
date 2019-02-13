// Fill out your copyright notice in the Description page of Project Settings.

#include "BaseEnemyAI.h"
#include "EnemyCharacter.h"
#include "PlayerCharacter.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMathLibrary.h"

// Perceptions
#include "Perception/AIPerceptionComponent.h"
#include "Perception/AISenseConfig_Sight.h"

// AI States
#include "BehaviorTree/BehaviorTree.h"
#include "BehaviorTree/BehaviorTreeComponent.h"
#include "BehaviorTree/BlackboardComponent.h"

#include "Runtime/Engine/Public/DrawDebugHelpers.h"

ABaseEnemyAI::ABaseEnemyAI()
{
	PrimaryActorTick.bCanEverTick = true;

	SightConfig = CreateDefaultSubobject<UAISenseConfig_Sight>(TEXT("Sight Config"));
	SetPerceptionComponent(*CreateDefaultSubobject<UAIPerceptionComponent>(TEXT("Perception Component")));

	// Default Perception Values
	SightConfig->SightRadius = AISightRadius;
	SightConfig->LoseSightRadius = AILoseSightRadius;
	SightConfig->PeripheralVisionAngleDegrees = AIFieldOfView;
	SightConfig->SetMaxAge(AISightAge);

	SightConfig->DetectionByAffiliation.bDetectEnemies = true;
	SightConfig->DetectionByAffiliation.bDetectEnemies = true;
	SightConfig->DetectionByAffiliation.bDetectNeutrals = true;

	GetPerceptionComponent()->SetDominantSense(*SightConfig->GetSenseImplementation());
	GetPerceptionComponent()->OnPerceptionUpdated.AddDynamic(this, &ABaseEnemyAI::OnPawnDetected);
	GetPerceptionComponent()->ConfigureSense(*SightConfig);

	BehaviorComp = CreateDefaultSubobject<UBehaviorTreeComponent>(TEXT("BehaviorComp"));
	BlackboardComp = CreateDefaultSubobject<UBlackboardComponent>(TEXT("BlackboardComp"));
}

void ABaseEnemyAI::BeginPlay()
{
	Super::BeginPlay();

	PlayerRef = Cast<APlayerCharacter>(UGameplayStatics::GetPlayerCharacter(GetWorld(), 0));

	if (GetPerceptionComponent())
	{
		UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] Begin Play!"));
	}
	
	// Initialize Enemy Detection Range
	FAISenseID SenseID = UAISense::GetSenseID<UAISense_Sight>();
	SightConfig = Cast<UAISenseConfig_Sight>(GetPerceptionComponent()->GetSenseConfig(SenseID));
	if (EnemyBot && SightConfig)
	{
		SightConfig->SightRadius = EnemyBot->EnemySightRadius;
		SightConfig->LoseSightRadius = EnemyBot->EnemySightRadius + 250.0f;
		GetPerceptionComponent()->ConfigureSense(*SightConfig);
	}

}

void ABaseEnemyAI::Possess(APawn* Pawn)
{
	Super::Possess(Pawn);

	EnemyBot = Cast<AEnemyCharacter>(Pawn);
	if (EnemyBot && EnemyBot->BehaviorTree)
	{
		if (EnemyBot->BehaviorTree->BlackboardAsset)
		{
			BlackboardComp->InitializeBlackboard(*EnemyBot->BehaviorTree->BlackboardAsset);
		}
		BehaviorComp->StartTree(*EnemyBot->BehaviorTree);

		// For testing
		UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] Behavior Tree Started!"));
		Patrol();
	}
}

void ABaseEnemyAI::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);

	//UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] Player Location: %s"), *PlayerRef->GetActorLocation().ToString());
}

FRotator ABaseEnemyAI::GetControlRotation() const
{
	if (!GetPawn())
	{
		return FRotator(0.0f, 0.0f, 0.0f);
	}
	return FRotator(0.0f, GetPawn()->GetActorRotation().Yaw, 0.0f);
}

void ABaseEnemyAI::OnPawnDetected(const TArray<AActor*> &DetectedPawns)
{
	for (int32 PawnIndex = 0; PawnIndex < DetectedPawns.Num(); PawnIndex++)
	{
		//APlayerCharacter* DetectedPlayer = Cast<APlayerCharacter>(DetectedPawns[PawnIndex]);
		if (DetectedPawns[PawnIndex] == Cast<AActor>(PlayerRef))
		{
			Chase();
			//Chase();
			//UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] Distance: %f"), FVector::Dist(EnemyBot->GetActorLocation(), PlayerRef->GetActorLocation()));
			//if (FVector::Dist(EnemyBot->GetActorLocation(), PlayerRef->GetActorLocation()) > EnemyBot->EnemySightRadius)
			//{
			//	Idle();
			//}
			//else
			//{
			//	Chase();
			//}
			// If Distance is within range, chase
			// Else stop
		}
	}
}

#pragma region Idle
void ABaseEnemyAI::Idle()
{
	BehaviorComp->RestartTree();
	EnemyBot->CurrentState = EnemyState::IDLE;
	BlackboardComp->SetValueAsString("CurrentState", "IDLE");
}
#pragma endregion

#pragma region Patrol
void ABaseEnemyAI::Patrol() // Proceed to next waypoint if bot has waypoints
{
	if (EnemyBot->Waypoints.Num() > 0)
	{
		CurrentWaypoint++;
		EnemyBot->CurrentState = EnemyState::PATROL;
		BlackboardComp->SetValueAsString("CurrentState", "PATROL");
		if (CurrentWaypoint >= EnemyBot->Waypoints.Num())
		{
			CurrentWaypoint = 0;
			BlackboardComp->SetValueAsVector("TargetDestination", EnemyBot->Waypoints[0]->GetActorLocation());
		}
		else
		{
			BlackboardComp->SetValueAsVector("TargetDestination", EnemyBot->Waypoints[CurrentWaypoint]->GetActorLocation());
		}
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] No Waypoints Found"));
	}
}
#pragma endregion

#pragma region Chase
void ABaseEnemyAI::Chase()
{
	if (EnemyBot->CurrentState != EnemyState::CHASE || EnemyBot->CurrentState == EnemyState::COMBAT)
	{
		BehaviorComp->RestartTree();
		EnemyBot->CurrentState = EnemyState::CHASE;
		BlackboardComp->SetValueAsString("CurrentState", "CHASE");
		BlackboardComp->SetValueAsObject("TargetPlayer", PlayerRef);
	}
}
#pragma endregion

#pragma region Investigate
void ABaseEnemyAI::Investigate()
{

}
#pragma endregion

#pragma region Combat
void ABaseEnemyAI::Combat()
{
	// Set Cone Check parameters
	BlackboardComp->SetValueAsVector("AILocation", EnemyBot->GetActorLocation());
	BlackboardComp->SetValueAsVector("ConeCheckDirection", EnemyBot->GetActorForwardVector());
	BlackboardComp->SetValueAsVector("PlayerLocation", PlayerRef->GetActorLocation());
	BehaviorComp->RestartTree();
	EnemyBot->CurrentState = EnemyState::COMBAT;
	BlackboardComp->SetValueAsString("CurrentState", "COMBAT");
}
#pragma endregion

#pragma region AI Utilities
void ABaseEnemyAI::UpdateLookDirection()
{
	//EnemyBot->lookatrotation
	FRotator Direction = UKismetMathLibrary::FindLookAtRotation(EnemyBot->GetActorLocation(), PlayerRef->GetActorLocation());
	EnemyBot->SetActorRotation(FMath::Lerp(EnemyBot->GetActorRotation(), Direction, 1.0f));
	BlackboardComp->SetValueAsVector("AILocation", EnemyBot->GetActorLocation());
	BlackboardComp->SetValueAsVector("ConeCheckDirection", EnemyBot->GetActorForwardVector());
	BlackboardComp->SetValueAsVector("PlayerLocation", PlayerRef->GetActorLocation());
}

// Used for BT Service for stopping MoveTo and starting combat
bool ABaseEnemyAI::WithinCombatRange()
{
//	UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] Combat Distance: %f"), FVector::Dist(EnemyBot->GetActorLocation(), PlayerRef->GetActorLocation()));
//	UE_LOG(LogTemp, Warning, TEXT("[BaseEnemyAI] Combat Distance 2: %f"), EnemyBot->EnemyAttackRange);
	//return FVector::Dist(EnemyBot->GetActorLocation(), PlayerRef->GetActorLocation()) <= ((EnemyBot->EnemyAttackRange * 2.0f) - EnemyBot->GetAttackRadius());
	return FVector::Dist(EnemyBot->GetActorLocation(), PlayerRef->GetActorLocation()) <= EnemyBot->EnemyAttackRange;
}
#pragma endregion


