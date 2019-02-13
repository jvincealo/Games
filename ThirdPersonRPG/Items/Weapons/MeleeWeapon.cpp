// Fill out your copyright notice in the Description page of Project Settings.

#include "MeleeWeapon.h"
#include "Kismet/GameplayStatics.h"
#include "Classes/Particles/ParticleSystem.h"
#include "Engine/World.h"
#include "Runtime/Engine/Public/DrawDebugHelpers.h"
#include "EnemyCharacter.h"

void AMeleeWeapon::TraceAttack()
{
	UE_LOG(LogTemp, Warning, TEXT("TRACE ATTACK"));
	UWorld* const World = GetWorld();
	if (World)
	{
		AActor* PlayerReference = GetOwner();
		TArray<FHitResult> HitResults;
		FCollisionQueryParams* TraceParams = new FCollisionQueryParams();
		FCollisionShape WeaponRadius = FCollisionShape::MakeSphere(AttackRadius);
		TraceParams->AddIgnoredActor(GetOwner());

		FVector traceEnd = PlayerReference->GetActorLocation() + (PlayerReference->GetActorForwardVector() * WeaponRange);
		FVector center = (PlayerReference->GetActorLocation() + traceEnd) / 2.0f;
		DrawDebugSphere(World, traceEnd, AttackRadius, 25, FColor(255, 0, 0), false, 10, 0, 2.0f);
		if (World->SweepMultiByChannel(HitResults, PlayerReference->GetActorLocation(), traceEnd, FQuat::Identity, ECC_Visibility, WeaponRadius, *TraceParams))
		{
			for(FHitResult hit : HitResults)
			{
				AEnemyCharacter* Enemy = Cast<AEnemyCharacter>(hit.GetActor());
				if (Enemy)
				{
					Enemy->UpdateHealth(-Damage);
				}
	
				UGameplayStatics::SpawnEmitterAtLocation(World, HitEffect, hit.Location);
				UE_LOG(LogTemp, Warning, TEXT("HIT: %s"), *hit.GetActor()->GetName());
			}
		}
	}
}

void AMeleeWeapon::EmitShockwave()
{

}


