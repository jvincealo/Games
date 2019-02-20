// Fill out your copyright notice in the Description page of Project Settings.

#include "PlayerCharacter.h"
#include "Components/SkeletalMeshComponent.h"
#include "Components/CapsuleComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "Camera/CameraComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMathLibrary.h"
#include "Blueprint/WidgetBlueprintLibrary.h"

// Combat
#include "Items/Weapons/BaseWeapon.h"
#include "Items/Weapons/RangedWeapon.h"
#include "Items/Weapons/MeleeWeapon.h"

// HUD
#include "Blueprint/UserWidget.h"
#include "HUD/WeaponWheelBase.h"

#include "Runtime/Engine/Public/DrawDebugHelpers.h"

APlayerCharacter::APlayerCharacter()
	: Super()
{
	PrimaryActorTick.bCanEverTick = true;

	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->TargetArmLength = 400.0f; // The camera follows at this distance behind the character	
	CameraBoom->bUsePawnControlRotation = true; // Rotate the arm based on the controller
	FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
	FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName);

	/* Load Player Stats Here */

	// Set initial value for movement inputs
	CrouchSpeed = 150.0f;
	WalkSpeed = 225.0;
	JogSpeed = 375.0f;
	SprintSpeed = 1200.0f;
}

void APlayerCharacter::BeginPlay()
{
	Super::BeginPlay();

	UseWeapon(0); // Initialize wit?h default weapon

	if (MainWidget)
	{
		MainHUD = CreateWidget<UUserWidget>(UGameplayStatics::GetPlayerController(GetWorld(), 0), MainWidget);
		MainHUD->AddToViewport();
	}
	if (WeaponSelectionWidget)
	{
		WeaponWheelHUD = CreateWidget<UUserWidget>(UGameplayStatics::GetPlayerController(GetWorld(), 0), WeaponSelectionWidget);
	}
}

#pragma region Player Stats
void APlayerCharacter::UpdateHealth(float p_health)
{
	// If incoming damage but invulnerable, don't proceed
	if (bInvulnerable && p_health < 0)
	{
		return;
	}
	Super::UpdateHealth(p_health);

	if (bAlive)
	{
		/*
		Player Death here
		*/
	}
}

void APlayerCharacter::UpdateExp(int32 p_exp)
{
	Exp += p_exp;

	if (Exp > Level * 1000) // 1000 for now, create required exp multiplier 
	{
		/*Level up */
	}
}

void APlayerCharacter::UpdateGold(int32 p_gold)
{
	Gold += p_gold;
}

void APlayerCharacter::UpdateMana(float p_mana)
{
	Mana += p_mana;
}

void APlayerCharacter::LevelUp()
{
	Level++;

	/* Additional bonuses here */
}
#pragma endregion

#pragma region Combat
void APlayerCharacter::SelectWeapon()
{
	// Fire up sheath weapon animation before showing weapon selection HUD
	bSelectingWeapon = !bSelectingWeapon;
	if (bSelectingWeapon) // Open Weapon Wheel and start weapon selection
	{
		if (WeaponWheelHUD)
		{
			WeaponWheelHUD->AddToViewport();
		}
	}
	else // Select weapon
	{
		if (WeaponWheelHUD)
			WeaponWheelHUD->RemoveFromParent();
		SelectedWeaponIndex = Cast<UWeaponWheelBase>(WeaponWheelHUD)->CurrentSelectedIndex;
	}
}


void APlayerCharacter::UseWeapon(int32 p_weapon)
{
	// Set Spawn Parameters for Weapon
	FVector SpawnLocation(0.0f, 0.0f, 0.0f);
	FRotator SpawnRotation(0.0f, 0.0f, 0.0f);
	FActorSpawnParameters SpawnParams;

	UWorld* const World = GetWorld();
	if (World)
	{
		CurrentWeapon = World->SpawnActor<ABaseWeapon>(WeaponList[p_weapon], SpawnLocation, SpawnRotation, SpawnParams);
		if (CurrentWeapon)
		{
			CurrentWeapon->SetOwner(this);
			CurrentWeapon->GetMesh()->SetupAttachment(RootComponent);
			// Check if weapon is Ranged or Melee
			ARangedWeapon* temp = Cast<ARangedWeapon>(CurrentWeapon);
			if (temp)
			{
				CurrentWeapon->AttachToComponent(GetMesh(), FAttachmentTransformRules(EAttachmentRule::SnapToTarget, true), TEXT("RangedSocket"));
			}
			else
			{
				AMeleeWeapon* temp = Cast<AMeleeWeapon>(CurrentWeapon);
				CurrentWeapon->AttachToComponent(GetMesh(), FAttachmentTransformRules(EAttachmentRule::SnapToTarget, true), TEXT("MeleeSocket"));
			}
		}
	}
}

void APlayerCharacter::Attack()
{
	UE_LOG(LogTemp, Warning, TEXT("ATTACK!!!!"));
	ARangedWeapon* temp = Cast<ARangedWeapon>(CurrentWeapon); // check first if equipped weapon is Melee or Ranged
	if (temp) // Ranged attack
	{
		if (bAim)
		{
			Cast<ARangedWeapon>(CurrentWeapon)->TraceProjectilePath(FollowCamera);
		}
	}
	else // Melee attacks
	{
		if (MeleeComboCounter > 3) MeleeComboCounter = 0; // For testing, diff weapons can have more than 3-hit combos
		bAttack = true;
	}
}

void APlayerCharacter::Reload()
{

}

void APlayerCharacter::ToggleAim(FVector p_zoomInOffset, FVector p_zoomOutOffset)
{
	ARangedWeapon* IsRanged = Cast<ARangedWeapon>(CurrentWeapon);
	if (IsRanged)
	{
		// If Player is currently sprinting, cancel sprint to walk slower and aim
		if (bSprint)
		{
			ToggleSprint();
		}
		bAim = !bAim;
		GetCharacterMovement()->MaxWalkSpeed = JogSpeed;
		OrientCharacter();
	}
}

void APlayerCharacter::TogglePerfectDodge(bool p_perfectDodge)
{
	bPerfectDodgeActive = p_perfectDodge;
}

void APlayerCharacter::Dodge()
{
	bDodge = true;
	if (bPerfectDodgeActive && !bInvulnerable)
	{
		PerfectDodge();
		UE_LOG(LogTemp, Warning, TEXT("PERFECT DODGE"));
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("NORMAL DODGE"));
	}
	bPerfectDodgeActive = false;
}

void APlayerCharacter::Parry()
{

}
#pragma endregion

#pragma region Items
void APlayerCharacter::UseItem()
{
	// Add class Item as parameter during implementation
}

void APlayerCharacter::PickupItem()
{

}
#pragma endregion

#pragma region Skills
void APlayerCharacter::UseSkill()
{
	// Add class Skill as parameter during implementation
}
#pragma endregion

#pragma region Movement
void APlayerCharacter::ToggleSprint()
{
	bSprint = !bSprint;
	if (bSprint)
	{
		APlayerCharacter::GetCharacterMovement()->MaxWalkSpeed = SprintSpeed;
	}
	else
	{
		APlayerCharacter::GetCharacterMovement()->MaxWalkSpeed = JogSpeed;
	}
}

void APlayerCharacter::ToggleCrouch()
{
	bSprint = !bSprint;
	bCrouch = !bCrouch;
	// If Player is currently sprinting, cancel sprint to crouch
	if (bSprint)
	{
		bSprint = false;
	}
	//decrease walk speed when crouching, set to default (jog speed) when standing
	if (bCrouch)
	{
		// Cancel Sprint if Crouch was used while Sprinting
		if (bSprint)
		{
			ToggleSprint();
		}
		GetCharacterMovement()->MaxWalkSpeed = CrouchSpeed;
	}
	else
	{
		GetCharacterMovement()->MaxWalkSpeed = JogSpeed;
	}
}
#pragma endregion

#pragma region Misc
void APlayerCharacter::SmoothZoom(FVector p_zoomInOffset, FVector p_zoomOutOffset)
{
	float TargetLength;
	if (bAim)
	{
		TargetLength = 150.0f;
		CameraBoom->SocketOffset = UKismetMathLibrary::VInterpTo(p_zoomInOffset, p_zoomOutOffset, 1.0f, 0.25f);
	}
	else
	{
		TargetLength = 400.0f;
		CameraBoom->SocketOffset = UKismetMathLibrary::VInterpTo(p_zoomOutOffset, p_zoomInOffset, 1.0f, 0.25f);
	}
	CameraBoom->TargetArmLength = UKismetMathLibrary::FInterpTo(CameraBoom->TargetArmLength, TargetLength, 1.0f, 0.25f);
}

void APlayerCharacter::OrientCharacter()
{
	if (bAim)
	{
		GetCharacterMovement()->bOrientRotationToMovement = false;
		GetCharacterMovement()->bUseControllerDesiredRotation = true;
	}
	else
	{
		GetCharacterMovement()->bOrientRotationToMovement = true;
		GetCharacterMovement()->bUseControllerDesiredRotation = false;
	}
}

#pragma endregion
