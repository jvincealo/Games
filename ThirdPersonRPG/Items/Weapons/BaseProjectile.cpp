// Fill out your copyright notice in the Description page of Project Settings.

#include "BaseProjectile.h"
#include "Components/StaticMeshComponent.h"
#include "Components/SphereComponent.h"
#include "Components/CapsuleComponent.h"
#include "GameFramework/ProjectileMovementComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Classes/Particles/ParticleSystemComponent.h"

#include "PlayerCharacter.h"
#include "EnemyCharacter.h"

// Sets default values
ABaseProjectile::ABaseProjectile()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	CollisionComp = CreateDefaultSubobject<USphereComponent>(TEXT("SphereCollision"));
	RootComponent = CollisionComp;
	PredictCollisionComp = CreateDefaultSubobject<UCapsuleComponent>(TEXT("PredictSphereCollision"));
	PredictCollisionComp->SetupAttachment(RootComponent);
	ProjectileMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("ProjectileMesh"));
	ProjectileMesh->SetupAttachment(RootComponent);
	ProjectileMovement = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("ProjectileMovement"));

	CollisionComp->OnComponentHit.AddDynamic(this, &ABaseProjectile::OnHit);
	PredictCollisionComp->OnComponentBeginOverlap.AddDynamic(this, &ABaseProjectile::OnBeginOverlap);
	PredictCollisionComp->OnComponentEndOverlap.AddDynamic(this, &ABaseProjectile::OnEndOverlap);

	// Default Values
	this->InitialLifeSpan = 5.0f;

	CollisionComp->InitSphereRadius(10.0f);
	CollisionComp->BodyInstance.SetCollisionProfileName("Projectile");
	CollisionComp->SetNotifyRigidBodyCollision(true);
	ProjectileMesh->bCastDynamicShadow = false;
	ProjectileMesh->CastShadow = false;
	ProjectileMovement->UpdatedComponent = CollisionComp;
	ProjectileMovement->InitialSpeed = 1500.0f;
	ProjectileMovement->MaxSpeed = 1500.0f;
	ProjectileMovement->ProjectileGravityScale = 0.0f;
}

// Called when the game starts or when spawned
void ABaseProjectile::BeginPlay()
{
	Super::BeginPlay();
	
}

void ABaseProjectile::OnHit(UPrimitiveComponent* HitComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, FVector NormalImpulse, const FHitResult& Hit)
{	
	// Projectile is fired by player, damage enemy
	if (ProjectileOwner->GetClass()->IsChildOf(APlayerCharacter::StaticClass()))
	{
		AEnemyCharacter* Enemy = Cast<AEnemyCharacter>(OtherActor);
		if (Enemy)
		{
			Enemy->UpdateHealth(-ABaseProjectile::Damage);
		}
	}
	else if(ProjectileOwner->GetClass()->IsChildOf(AEnemyCharacter::StaticClass()))
	{
		APlayerCharacter* Player = Cast<APlayerCharacter>(OtherActor);
		if (Player)
		{
			if (!Player->IsInvulnerable())
			{
				Player->UpdateHealth(-ABaseProjectile::Damage);
				Player->TogglePerfectDodge(false);
				UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), HitEffect, Hit.Location);
			}
		}
	}
	else
	{
		UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), HitEffect, Hit.Location);
	}

	
	Destroy();
}

void ABaseProjectile::OnBeginOverlap(UPrimitiveComponent * OverlappedComponent, AActor * OtherActor, UPrimitiveComponent * OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult & SweepResult)
{	
	UE_LOG(LogTemp, Warning, TEXT("OVERLAP!"));
	APlayerCharacter* Player = Cast<APlayerCharacter>(OtherActor);
	if(Player)
	{
		UE_LOG(LogTemp, Warning, TEXT("OVERLAP2!"));
		Player->TogglePerfectDodge(true);
	}
}

void ABaseProjectile::OnEndOverlap(UPrimitiveComponent * OverlappedComp, AActor * OtherActor, UPrimitiveComponent * OtherComp, int32 OtherBodyIndex)
{
	APlayerCharacter* Player = Cast<APlayerCharacter>(OtherActor);
	if (Player)
	{
		Player->TogglePerfectDodge(false);
	}
}

// Called every frame
void ABaseProjectile::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void ABaseProjectile::InitVelocity(const FVector& p_direction)
{
	if (ProjectileMovement)
	{
		ProjectileMovement->Velocity = p_direction * ProjectileMovement->InitialSpeed;
	}
}

void ABaseProjectile::InitProjectileInfo(UParticleSystem* p_particles, float p_damage)
{
	HitEffect = p_particles;
	Damage = p_damage;
}