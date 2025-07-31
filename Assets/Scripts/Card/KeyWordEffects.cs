using UnityEngine;
using System.Collections.Generic;

namespace Effects
{
    public static class KeywordEffects
    {
        /// <summary>
        /// 根据玩家 targetAttackPosition 判断该格是否存在怪物，若存在返回该怪物，否则返回 null
        /// </summary>
        public static Monster GetMonsterAtPosition(Vector2Int pos)
        {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            foreach (GameObject monsterObject in monsters)
            {
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(pos))
                {
                    return monster;
                }
            }
            return null;
        }

        /// <summary>
        /// 将任意方向向量转换为最接近的四个正方向（上、下、左、右）
        /// </summary>
        public static Vector2 RoundToCardinal(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                return new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                return new Vector2(0, Mathf.Sign(direction.y));
            }
        }

        /// <summary>
        /// 对目标怪物应用击退效果：
        /// 尝试将其沿指定方向移动 1 格，
        /// 如果目标格超出棋盘范围或被阻挡，则对怪物造成 1 点伤害。
        /// </summary>
        /// <param name="target">目标怪物</param>
        /// <param name="direction">击退方向（应为 (1,0), (-1,0), (0,1) 或 (0,-1)）</param>
        /// <param name="player">玩家对象，用于判断目标位置是否合法</param>
        public static void ApplyKnockback(Monster target, Vector2 direction, Player player)
        {
            Vector2Int currentPos = target.position;
            Vector2Int knockbackDir = new Vector2Int((int)direction.x, (int)direction.y);
            Vector2Int desiredPos = currentPos + knockbackDir;
            
            if (IsPositionValid(desiredPos, player))
            {
                target.position = desiredPos;
                target.UpdatePosition();
            }
            else
            {
                target.TakeDamage(1);
            }
        }

        /// <summary>
        /// 判断目标位置是否有效：
        /// 1. 必须在棋盘范围内（player.IsValidPosition）
        /// 2. 该位置未被怪物或不可进入的地形阻挡（!player.IsBlockedBySomething）
        /// </summary>
        private static bool IsPositionValid(Vector2Int pos, Player player)
        {
            return player.IsValidPosition(pos) && !player.IsBlockedBySomething(pos);
        }

        /// <summary>
        /// 封装攻击并击退的完整效果：
        /// 1. 根据玩家的 targetAttackPosition 获取目标怪物
        /// 2. 计算击退方向（从玩家到怪物方向，再转换为上下左右方向）
        /// 3. 应用击退效果
        /// </summary>
        /// <param name="player">玩家对象，必须包含 targetAttackPosition、IsValidPosition、IsBlockedBySomething 等方法</param>
        public static void AttackWithKnockback(Player player, Vector2Int attackPos)
        {
            // 根据玩家的攻击目标位置判断是否存在怪物
            Monster targetMonster = GetMonsterAtPosition(attackPos);
            if (targetMonster != null)
            {
                // 计算方向：从玩家到目标怪物
                Vector2 direction = (targetMonster.transform.position - player.transform.position).normalized;
                // 转换为卡尔迪纳方向
                Vector2 cardinalDirection = RoundToCardinal(direction);
                // 应用击退效果（尝试移动 1 格）
                ApplyKnockback(targetMonster, cardinalDirection, player);
            }
        }

        // -------------------------------------
        // Basic Ritual Logic (基础仪式计数)
        // -------------------------------------

        private static bool ritualActive = false;
        private static int ritualCount = 0;
        private const int RitualTarget = 2;

        /// <summary>
        /// 启动基础仪式：仅在首次调用时激活。
        /// </summary>
        public static void StartBasicRitual()
        {
            if (ritualActive) return;
            ritualActive = true;
            ritualCount = 0;
            Debug.Log("Basic ritual started.");
        }

        /// <summary>
        /// 累计一次仪式进度，达到目标后自动完成仪式。
        /// </summary>
        public static void IncrementBasicRitual()
        {
            if (!ritualActive) return;
            ritualCount++;
            Debug.Log($"Basic ritual progress: {ritualCount}/{RitualTarget}");
            if (ritualCount >= RitualTarget)
                CompleteRitual();
        }

        /// <summary>
        /// 完成仪式：对所有敌人造成 1 点伤害，并重置状态。
        /// </summary>
        private static void CompleteRitual()
        {
            ritualActive = false;
            ritualCount = 0;
            Debug.Log("Basic ritual completed! Dealing 1 damage to all monsters.");
            foreach (var monster in GameObject.FindObjectsOfType<Monster>())
                monster.TakeDamage(1);
        }
        public static void StopBasicRitual()
        {
            // 关闭仪式并清零计数
            ritualActive = false;
            ritualCount  = 0;
            Debug.Log("Basic ritual stopped and reset.");
        }
        
        // -------------------------------------
        // BS系列卡牌特殊效果 (BS Card Special Effects)
        // -------------------------------------
        
        /// <summary>
        /// BS06狮鹫势效果：使下一张武器牌使用2次
        /// </summary>
        public static void ActivateGriffinStance(Player player)
        {
            if (player != null)
            {
                player.nextWeaponCardDoubleUse = true;
                Debug.Log("BS06 Griffin Stance: Next weapon card will be used twice");
            }
        }
        
        /// <summary>
        /// BS07卷轴匣效果：使下一张卡牌回到牌库顶部
        /// </summary>
        public static void ActivateScrollCase(Player player)
        {
            if (player != null)
            {
                player.nextCardReturnToDeckTop = true;
                Debug.Log("BS07 Scroll Case: Next card will return to deck top");
            }
        }
        
        /// <summary>
        /// BS08盐袋效果：激活每个敌人死亡获得护甲的效果
        /// </summary>
        public static void ActivateSaltBag(Player player)
        {
            if (player != null)
            {
                player.saltBagEffectActive = true;
                Debug.Log("BS08 Salt Bag: Will gain armor for each enemy death");
            }
        }
        
        /// <summary>
        /// 处理敌人死亡时的盐袋效果
        /// </summary>
        public static void TriggerSaltBagOnEnemyDeath(Player player, string enemyName)
        {
            if (player != null && player.saltBagEffectActive)
            {
                player.AddArmor(1);
                Debug.Log($"BS08 Salt Bag effect: Gained 1 armor from {enemyName} death");
            }
        }
        
        /// <summary>
        /// BS09某种草药效果：当失去所有行动点时，从牌库或弃牌堆回到手牌
        /// </summary>
        public static void TriggerHerbOnNoActions(Player player)
        {
            if (player == null) return;
            
            DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
            if (deckManager == null) return;
            
            // 先从牌库中寻找BS09
            Card herbCard = null;
            for (int i = 0; i < deckManager.deck.Count; i++)
            {
                if (deckManager.deck[i].Id == "BS09")
                {
                    herbCard = deckManager.deck[i];
                    deckManager.deck.RemoveAt(i);
                    Debug.Log("BS09: Found herb in deck, moving to hand");
                    break;
                }
            }
            
            // 如果牌库没有，从弃牌堆寻找
            if (herbCard == null)
            {
                for (int i = 0; i < deckManager.discardPile.Count; i++)
                {
                    if (deckManager.discardPile[i].Id == "BS09")
                    {
                        herbCard = deckManager.discardPile[i];
                        deckManager.discardPile.RemoveAt(i);
                        Debug.Log("BS09: Found herb in discard pile, moving to hand");
                        break;
                    }
                }
            }
            
            // 如果找到了，添加到手牌
            if (herbCard != null)
            {
                deckManager.DrawSpecificCard(herbCard);
                Debug.Log("BS09: Herb card returned to hand due to no actions");
            }
        }
        
        /// <summary>
        /// 恩赐效果：抽取1张卡牌
        /// </summary>
        public static void TriggerGraceEffect()
        {
            DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
            if (deckManager != null)
            {
                deckManager.DrawCards(1);
                Debug.Log("Grace effect: Drew 1 card");
            }
        }
        
        /// <summary>
        /// 涌潮效果：使用移动牌后对邻近敌人造成伤害
        /// </summary>
        public static void TriggerTorrentEffect(Player player, Vector2Int targetPosition)
        {
            if (player.torrentStacks <= 0) return;
            
            // 获取目标位置邻近的敌人
            List<Monster> adjacentMonsters = new List<Monster>();
            Vector2Int[] adjacentPositions = {
                targetPosition + Vector2Int.up,
                targetPosition + Vector2Int.down,
                targetPosition + Vector2Int.left,
                targetPosition + Vector2Int.right
            };
            
            foreach (Vector2Int pos in adjacentPositions)
            {
                Monster monster = GetMonsterAtPosition(pos);
                if (monster != null)
                {
                    adjacentMonsters.Add(monster);
                }
            }
            
            if (adjacentMonsters.Count > 0)
            {
                // 根据涌潮层数触发多次
                for (int i = 0; i < player.torrentStacks; i++)
                {
                    Monster target = adjacentMonsters[Random.Range(0, adjacentMonsters.Count)];
                    target.TakeDamage(1);
                    Debug.Log($"Torrent effect {i + 1}/{player.torrentStacks}: Dealt 1 damage to {target.monsterName}");
                }
            }
        }
        
        /// <summary>
        /// 逐浪效果：移动后击退目标点3x3范围内随机敌人
        /// </summary>
        public static void TriggerWaveRiderEffect(Player player, Vector2Int targetPosition)
        {
            if (!player.waveRiderEffectActive) return;
            
            // 获取目标位置3x3范围内的敌人
            List<Monster> nearbyMonsters = new List<Monster>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector2Int checkPos = targetPosition + new Vector2Int(dx, dy);
                    Monster monster = GetMonsterAtPosition(checkPos);
                    if (monster != null)
                    {
                        nearbyMonsters.Add(monster);
                    }
                }
            }
            
            if (nearbyMonsters.Count > 0)
            {
                // 随机选择一个敌人进行击退
                Monster target = nearbyMonsters[Random.Range(0, nearbyMonsters.Count)];
                Vector2 direction = (target.transform.position - player.transform.position).normalized;
                Vector2 cardinalDirection = RoundToCardinal(direction);
                ApplyKnockback(target, cardinalDirection, player);
                Debug.Log($"Wave Rider effect: Knocked back {target.monsterName}");
            }
        }
        
        /// <summary>
        /// WS01回响效果：在玩家3x3范围内创造潮沼
        /// </summary>
        public static void TriggerWS01EchoEffect(Player player)
        {
            LocationManager locationManager = GameObject.FindObjectOfType<LocationManager>();
            if (locationManager != null && player != null)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Vector2Int checkPos = player.position + new Vector2Int(dx, dy);
                        
                        if (player.IsValidPosition(checkPos) && 
                            !locationManager.IsNonEnterablePosition(checkPos) &&
                            checkPos != player.position)
                        {
                            locationManager.CreateMire(checkPos);
                        }
                    }
                }
                Debug.Log("WS01: Echo effect - created mires in 3x3 area");
            }
        }
        
        /// <summary>
        /// 重置所有BS系列效果（回合结束时调用）
        /// </summary>
        public static void ResetBSEffects(Player player)
        {
            if (player != null)
            {
                player.nextWeaponCardDoubleUse = false;
                player.nextCardReturnToDeckTop = false;
                // 注意：盐袋效果和逐浪效果不在回合结束时重置，因为它们是整个战斗持续的
                Debug.Log("BS Effects reset (except Salt Bag and Wave Rider which persist through battle)");
            }
        }
    }
}
