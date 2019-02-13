// Fill out your copyright notice in the Description page of Project Settings.

#include "TASK_MOVE.h"
#include "AIController.h"

#include "EnemyCharacter.h"
#include "PlayerCharacter.h"
#include "Kismet/GameplayStatics.h"
#include "Engine/World.h"

EBTNodeResult::Type UTASK_MOVE::ExecuteTask(UBehaviorTreeComponent & OwnerComp, uint8 * NodeMemory)
{
	AEnemyCharacter* EnemyOwner = Cast<AEnemyCharacter>(OwnerComp.GetAIOwner()->GetPawn());
	APlayerCharacter* PlayerRef = Cast<APlayerCharacter>(UGameplayStatics::GetPlayerCharacter(GetWorld(), 0));

	// Set custom parameters based on enemy and player stats
	if (EnemyOwner && PlayerRef)
	{
		AcceptableRadius = EnemyOwner->EnemyAttackRange;
		bReachTestIncludesGoalRadius = bReachTestIncludesAgentRadius = bStopOnOverlap = false;
	}
	return Super::ExecuteTask(OwnerComp, NodeMemory);
}
