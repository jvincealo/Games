// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BaseCharacter.h"
#include "PlayerCharacter.generated.h"

/**
 * General controls and movement will be implemented with Blueprints
 */
UCLASS()
class THIRDPERSONRPG_API APlayerCharacter : public ABaseCharacter
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
		class USpringArmComponent* CameraBoom;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
		class UCameraComponent* FollowCamera;

public:
	APlayerCharacter();

	UFUNCTION(BlueprintCallable, Category = "Player Stats")
		void UpdateHealth(float p_health) override;

	UFUNCTION(BlueprintCallable, Category = "Player Stats")
		void UpdateExp(int32 p_exp);

	UFUNCTION(BlueprintCallable, Category = "Player Stats")
		void UpdateGold(int32 p_gold);

	UFUNCTION(BlueprintCallable, Category = "Player Stats")
		void UpdateMana(float p_mana);

	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void TogglePerfectDodge(bool p_perfectDodge);

protected:
	virtual void BeginPlay() override;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player HUD")
		TSubclassOf<class UUserWidget> MainWidget;
	// Contains HP bar and other main UI
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Player HUD")
		UUserWidget* MainHUD;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player HUD")
		TSubclassOf<class UUserWidget> WeaponSelectionWidget;
	
	// Contains the Weapon selection wheel
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Player HUD")
		UUserWidget* WeaponWheelHUD;
	
protected:
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Movement")
		bool bCrouch;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Movement")
		bool bDodge;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Movement")
		bool bSprint;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Combat")
		bool bAim;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Combat")
		bool bAttack;

	// Keeps track of current melee combo count to execute corresponding melee attack animation
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Combat")
		int32 MeleeComboCounter;

	// Whether there's an incoming attack on the player (for perfectly timed dodges)
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Combat")
		bool bPerfectDodgeActive;

	// Whether the player can take damage or not
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Combat")
		bool bInvulnerable;

	//very slow walking speed 
	UPROPERTY(EditAnywhere, Category = "Player Movement")
		float CrouchSpeed;

	//slightly low walking speed for aiming
	UPROPERTY(EditAnywhere, Category = "Player Movement")
		float WalkSpeed;

	//default walking speed
	UPROPERTY(EditAnywhere, Category = "Player Movement")
		float JogSpeed; 

	 //running speed
	UPROPERTY(EditAnywhere, Category = "Player Movement")
		float SprintSpeed;

	UPROPERTY(EditAnywhere, Category = "Player Stats")
		int32 Level = 1;

	UPROPERTY(EditAnywhere, Category = "Player Stats")
		int32 Exp = 0;

	UPROPERTY(EditAnywhere, Category = "Player Stats")
		int32 Gold = 100;

	UPROPERTY(EditAnywhere, Category = "Player Stats")
		int32 Mana = 100;


protected:

	int32 CurrentWeaponIndex;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Character")
		bool bSelectingWeapon;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Player Character")
		int32 SelectedWeaponIndex;

	//UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Player Character")
	// SkillList

	//UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Player Character")
	// Items

	UFUNCTION(BlueprintCallable, Category = "Player Stats")
		void LevelUp();

	// Brings up the HUD for Radial Menu Weapon Selection
	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void SelectWeapon(); 

	// Equips the weapon selected from the HUD
	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void UseWeapon(int32 p_weapon);

	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void Attack() override;
	
	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void Reload();

	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void ToggleAim(FVector p_zoomInOffset, FVector p_zoomOutOffset);

	UFUNCTION(BlueprintImplementableEvent, Category = "Player Combat")
		void PerfectDodge();

	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void Dodge();

	UFUNCTION(BlueprintCallable, Category = "Player Combat")
		void Parry();

	// Add class Item as parameter during implementation
	UFUNCTION(BlueprintCallable, Category = "Player Character")
		void UseItem();

	UFUNCTION(BlueprintCallable, Category = "Player Character")
		void PickupItem();

	// Add class Skill as parameter during implementation
	UFUNCTION(BlueprintCallable, Category = "Player Character")
		void UseSkill();

	UFUNCTION(BlueprintCallable, Category = "Player Movement")
		void ToggleSprint();

	UFUNCTION(BlueprintCallable, Category = "Player Movement")
		void ToggleCrouch();

protected:
	// Interpolates Camera's Arm Length to create a smooth zooming effect rather than instant zoom
	UFUNCTION(BlueprintCallable, Category = "Helper Functions")
		void SmoothZoom(FVector p_zoomInOffset, FVector p_zoomOutOffset);

	// Adjust player rotation to camera depending if aiming or not
	UFUNCTION(BlueprintCallable, Category = "Helper Functions")
		void OrientCharacter();

public:
	/** Returns CameraBoom subobject **/
	FORCEINLINE class USpringArmComponent* GetCameraBoom() const { return CameraBoom; }

	bool IsInvulnerable() { return bInvulnerable; }

};
