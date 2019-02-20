// Fill out your copyright notice in the Description page of Project Settings.

#include "RangedWeapon.h"
#include "Components/SkeletalMeshComponent.h"
#include "Camera/CameraComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Runtime/Engine/Public/DrawDebugHelpers.h"
#include "EnemyCharacter.h"
#include "Classes/Particles/ParticleSystemComponent.h"

#include "BaseProjectile.h"


ARangedWeapon::ARangedWeapon()
{
	PrimaryActorTick.bCanEverTick = true;

	// Create bullet spawn point
	GunMuzzle = CreateDefaultSubobject<USceneComponent>(TEXT("GunMuzzle"));
	GunMuzzle->SetupAttachment(WeaponSkeletalMesh);
}

void ARangedWeapon::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (LaserSight)
	{
		UpdateLaserSightPosition();
	}
}

void ARangedWeapon::TraceProjectilePath(UCameraComponent* p_camera) // Trace from middle of camera first to acquire end point accurately
{
	UWorld* const World = GetWorld();
	if (World)
	{
		FHitResult* HitResult = new FHitResult();
		FCollisionQueryParams* TraceParams = new FCollisionQueryParams();

		FVector tempEnd = p_camera->GetComponentLocation() + (p_camera->GetForwardVector() * WeaponRange);

		//DrawDebugLine(World, p_camera->GetComponentLocation(), tempEnd, FColor(0, 0, 255), false, 2.0f, 0, 1.0f);
		// Raycast from the Gun muzzle and check for gunfire hit
		if (World->LineTraceSingleByChannel(*HitResult, p_camera->GetComponentLocation(), tempEnd, ECC_Camera, *TraceParams))
		{
			UGameplayStatics::SpawnEmitterAtLocation(World, HitEffect, HitResult->Location);
			AEnemyCharacter* Enemy = Cast<AEnemyCharacter>(HitResult->GetActor());
			if (Enemy)
			{
				TraceEnd = HitResult->Location + (GetOwner()->GetActorForwardVector() * 5);
				TraceAttack();
			}
		}
	}
}

void ARangedWeapon::TraceAttack() // Trace from gun muzzle to acquired end point
{
	if (bAutomatic)
	{
		UWorld* const World = GetWorld();
		if (World)
		{
			FHitResult* HitResult = new FHitResult();
			FCollisionQueryParams* TraceParams = new FCollisionQueryParams();

			DrawDebugLine(World, GunMuzzle->GetComponentLocation(), TraceEnd, FColor(0,255,0), false, 2.0f, 0, 1.0f);
			// Raycast from the Gun muzzle and check for gunfire hit
			if (World->LineTraceSingleByChannel(*HitResult, GunMuzzle->GetComponentLocation(), TraceEnd, ECC_Camera, *TraceParams))
			{
				UGameplayStatics::SpawnEmitterAtLocation(World, HitEffect, HitResult->Location);
				AEnemyCharacter* Enemy = Cast<AEnemyCharacter>(HitResult->GetActor());
				if (Enemy)
				{
					UE_LOG(LogTemp, Warning, TEXT("HIT: %s"), *HitResult->BoneName.ToString());
					if (HitResult->BoneName.ToString().ToLower().Equals("head"))
					{
						Enemy->UpdateHealth(-ARangedWeapon::Damage * 2.0f);
						UE_LOG(LogTemp, Warning, TEXT("HEADSHOT!!!"));
					}
					else
					{
						Enemy->UpdateHealth(-ARangedWeapon::Damage);
						UE_LOG(LogTemp, Warning, TEXT("NOT!"));
					}
				}
			}
		}
	}
}

void ARangedWeapon::FireProjectile()
{
	UWorld* const World = GetWorld();
	if (World)
	{
		FActorSpawnParameters SpawnParams;
		FVector SpawnLocation(0.0f, 0.0f, 0.0f);
		FRotator SpawnRotation(0.0f, 0.0f, 0.0f);
		ABaseProjectile* const Projectile = World->SpawnActor<ABaseProjectile>(WeaponProjectile, GunMuzzle->GetComponentLocation(), GunMuzzle->GetComponentRotation(), SpawnParams);
		if (Projectile)
		{
			Projectile->ProjectileOwner = GetOwner();
			Projectile->SetActorLocation(GunMuzzle->GetComponentLocation());
			FVector const LaunchDir = GunMuzzle->GetComponentRotation().Vector();
			Projectile->InitVelocity(LaunchDir);
			Projectile->InitProjectileInfo(HitEffect, Damage);
		}
	}
}

void ARangedWeapon::Reload()
{

}

void ARangedWeapon::SetLaserSight(UParticleSystem* p_particle)
{
	UWorld* const World = GetWorld();
	if (World)
	{
		LaserSight = UGameplayStatics::SpawnEmitterAtLocation(World, p_particle, GunMuzzle->GetComponentLocation());
		LaserSight->Activate();
		UpdateLaserSightPosition();
	}
}

void ARangedWeapon::UpdateLaserSightPosition()
{
	UWorld* const World = GetWorld();
	if (World)
	{
		LaserSight->SetBeamSourcePoint(0, GunMuzzle->GetComponentLocation(), 0);
		FHitResult* HitResult = new FHitResult();
		FCollisionQueryParams* TraceParams = new FCollisionQueryParams();

		FVector BeamTarget = GunMuzzle->GetComponentLocation() + (GunMuzzle->GetForwardVector() * WeaponRange);
		// Raycast from the Gun muzzle to get end point of beam
		if (World->LineTraceSingleByChannel(*HitResult, GunMuzzle->GetComponentLocation(), BeamTarget, ECC_Camera, *TraceParams))
		{
			LaserSight->SetBeamTargetPoint(0, HitResult->Location, 0);
		}
		else
		{
			LaserSight->SetBeamTargetPoint(0, BeamTarget, 0);
		}
	}
}

