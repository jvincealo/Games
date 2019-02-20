// Fill out your copyright notice in the Description page of Project Settings.

#include "BaseCharacter.h"

// Sets default values
ABaseCharacter::ABaseCharacter()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void ABaseCharacter::BeginPlay()
{
	Super::BeginPlay();
}

// Called every frame
void ABaseCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void ABaseCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

#pragma region Character Status
void ABaseCharacter::UpdateHealth(float p_health)
{
	Health += p_health;
	UE_LOG(LogTemp, Warning, TEXT("Health: %f"), Health);
	if (Health <= 0)
	{
		bAlive = false;
	}
}
#pragma endregion

#pragma region Combat
void ABaseCharacter::Attack(){}
#pragma endregion
