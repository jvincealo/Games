// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BaseProjectile.generated.h"

UCLASS()
class THIRDPERSONRPG_API ABaseProjectile : public AActor
{
	GENERATED_BODY()

protected:
	class UParticleSystem* HitEffect;

	UPROPERTY(EditAnywhere)
		class UStaticMeshComponent* ProjectileMesh;

	UPROPERTY(EditAnywhere)
		class USphereComponent* CollisionComp;

	UPROPERTY(EditAnywhere)
		class UCapsuleComponent* PredictCollisionComp;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = Movement)
		class UProjectileMovementComponent* ProjectileMovement;

	float Damage;
	
public:	
	// Sets default values for this actor's properties
	ABaseProjectile();

	UPROPERTY(VisibleAnywhere)
		AActor* ProjectileOwner;

	void InitVelocity(const FVector& p_direction);

	void InitProjectileInfo(UParticleSystem* p_particle, float p_damage);

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	UFUNCTION()
		void OnHit(UPrimitiveComponent* HitComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, FVector NormalImpulse, const FHitResult& Hit);

	UFUNCTION()
		void OnBeginOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult &SweepResult);

	UFUNCTION()
		void OnEndOverlap(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	
	
};
