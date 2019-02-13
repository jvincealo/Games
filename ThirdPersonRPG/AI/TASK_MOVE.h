// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BehaviorTree/Tasks/BTTask_MoveTo.h"
#include "TASK_MOVE.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API UTASK_MOVE : public UBTTask_MoveTo
{
	GENERATED_BODY()
	
	virtual EBTNodeResult::Type ExecuteTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory) override;
	
	
};
