// Fill out your copyright notice in the Description page of Project Settings.

#include "EnemyCharacter.h"
#include "Components/SkeletalMeshComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Engine/World.h"
#include "PlayerCharacter.h"

// Combat
#include "Items/Weapons/BaseWeapon.h"
#include "Items/Weapons/RangedWeapon.h"
#include "Items/Weapons/MeleeWeapon.h"

// AI States
#include "BehaviorTree/BehaviorTree.h"
#include "BehaviorTree/BehaviorTreeComponent.h"
#include "BehaviorTree/BlackboardComponent.h"

// VFX
#include "Classes/Particles/ParticleSystem.h"
#include "Classes/Particles/ParticleSystemComponent.h"

#include "Runtime/Engine/Public/DrawDebugHelpers.h"

AEnemyCharacter::AEnemyCharacter()
{
	// Init base stats here
}

void AEnemyCharacter::BeginPlay()
{
	Super::BeginPlay();

	InitializeEnemy();
}

void AEnemyCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if (bPredictAttackActive)
	{
		PredictAttackPath();
	}
}

void AEnemyCharacter::InitializeEnemy()
{
	// Set Spawn Parameters for Weapon
	FVector SpawnLocation(0.0f, 0.0f, 0.0f);
	FRotator SpawnRotation(0.0f, 0.0f, 0.0f);
	FActorSpawnParameters SpawnParams;

	UWorld* const World = GetWorld();
	if (World)
	{
		CurrentWeapon = World->SpawnActor<ABaseWeapon>(WeaponList[0], SpawnLocation, SpawnRotation, SpawnParams);
		if (CurrentWeapon)
		{
			CurrentWeapon->SetOwner(this);
			CurrentWeapon->GetMesh()->SetupAttachment(RootComponent);
			// Check if weapon is Ranged or Melee
			ARangedWeapon* temp = Cast<ARangedWeapon>(CurrentWeapon);
			if (temp)
			{
				CurrentWeapon->AttachToComponent(GetMesh(), FAttachmentTransformRules(EAttachmentRule::SnapToTarget, true), TEXT("RangedSocket"));
				if (LaserSight)
				{
					Cast<ARangedWeapon>(CurrentWeapon)->SetLaserSight(LaserSight);
				}
			}
			else
			{
				AMeleeWeapon* temp = Cast<AMeleeWeapon>(CurrentWeapon);
				CurrentWeapon->AttachToComponent(GetMesh(), FAttachmentTransformRules(EAttachmentRule::SnapToTarget, true), TEXT("MeleeSocket"));
			}
		}
	}
}

#pragma region Enemy Combat
APlayerCharacter* AEnemyCharacter::TraceAttack()
{
	UWorld* const World = GetWorld();
	if (World)
	{
		TArray<FHitResult> HitResults;
		FCollisionQueryParams* TraceParams = new FCollisionQueryParams();
		FCollisionShape WeaponRadius = FCollisionShape::MakeSphere(AEnemyCharacter::AttackRadius);
		TraceParams->AddIgnoredActor(this);
		FVector traceEnd = GetActorLocation() + (GetActorForwardVector() * AEnemyCharacter::EnemyAttackRange);
		FVector center = (GetActorLocation() + traceEnd) / 2.0f;
		DrawDebugSphere(World, traceEnd, AttackRadius, 25, FColor(255, 0, 0), false, 0.1f, 0, 2.0f);
		if (World->SweepMultiByChannel(HitResults, GetActorLocation(), traceEnd, FQuat::Identity, ECC_Visibility, WeaponRadius, *TraceParams))
		{
			for (FHitResult hit : HitResults)
			{
				APlayerCharacter* PlayerRef = Cast<APlayerCharacter>(hit.GetActor());
				if (PlayerRef)
				{
					return PlayerRef;
				}
				UE_LOG(LogTemp, Warning, TEXT("HIT: %s"), *hit.GetActor()->GetName());
			}
		}
	}
	return nullptr;
}

void AEnemyCharacter::Attack()
{
	ARangedWeapon* temp = Cast<ARangedWeapon>(CurrentWeapon);
	if (temp)
	{
		//Cast<ARangedWeapon>(CurrentWeapon)->FireProjectile();
		temp->FireProjectile();
	}
	else
	{
		APlayerCharacter* PlayerRef = AEnemyCharacter::TraceAttack();
		if (PlayerRef)
		{
			PlayerRef->TogglePerfectDodge(false);
			PlayerRef->UpdateHealth(-AEnemyCharacter::Damage);
		}
	}
}

void AEnemyCharacter::PredictAttackPath() // Line trace for player's perfectly timed dodges
{
	APlayerCharacter* PlayerRef = AEnemyCharacter::TraceAttack();
	if (PlayerRef)
	{
		UE_LOG(LogTemp, Warning, TEXT("TOGGLE!!!"));
		PlayerRef->TogglePerfectDodge(true);
	}
}

#pragma endregion
/*
--- Constructor ---
CharacterMovement:

OrientToMovement = true
RotationRate = FRotator(0, 600, 0)

*/