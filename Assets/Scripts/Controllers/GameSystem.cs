using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : PersistentSingleton<GameSystem>{
  [SerializeField] public AudioManager audioManager;
}