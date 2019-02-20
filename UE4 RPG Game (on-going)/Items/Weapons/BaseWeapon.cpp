// Fill out your copyright notice in the Description page of Project Settings.

#include "BaseWeapon.h"
#include "Components/SkeletalMeshComponent.h"

ABaseWeapon::ABaseWeapon()
{
	// Create Weapon Mesh
	WeaponSkeletalMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("GunMesh"));
	WeaponSkeletalMesh->bCastDynamicShadow = false;
	WeaponSkeletalMesh->CastShadow = false;
	RootComponent = WeaponSkeletalMesh;
}

void ABaseWeapon::TraceAttack(){}


