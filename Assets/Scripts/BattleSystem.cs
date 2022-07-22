using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	public AudioSource source;
	public AudioClip bleep;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public GameObject nameObject;
	public GameObject button;

	public GameObject loseThingy;
	public Text loseDialogue;
	public Text bullyName;

	public string text;

	// Start is called before the first frame update
	void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		text = "A wild " + enemyUnit.unitName + " approaches...";
		StartCoroutine(TypeSentence(text, dialogueText));


		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);

		//dialogueText.text = "The attack is successful!";
		StartCoroutine(TypeSentence("The attack is successful!", dialogueText));

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		text = enemyUnit.unitName + " attacks!";
		StartCoroutine(TypeSentence(text, dialogueText));

		yield return new WaitForSeconds(1f);

		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}

	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			StartCoroutine(TypeSentence("You won the battle!", dialogueText));
		}
		else if (state == BattleState.LOST)
		{
			OnEnemyWin();
			//dialogueText.text = "You were defeated.";
		}
	}

	void PlayerTurn()
	{
		StartCoroutine(TypeSentence("Choose an action:", dialogueText));
		//dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		//dialogueText.text = "You feel renewed strength!";
		StartCoroutine(TypeSentence("You feel renewed strength!", dialogueText));

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}


	public void OnEnemyWin()
    {
		dialogueText.text = "";
		button.SetActive(false);
		loseThingy.SetActive(true);
		bullyName.text = "Obama";
		StartCoroutine(TypeSentence("Dig a grave next to your dad, loser!", loseDialogue));
		//loseDialogue.text = "Dig a grave next to your dad, loser!";
    }


	IEnumerator TypeSentence(string sentence, Text textBox)
	{
		textBox.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			textBox.text += letter;
			yield return new WaitForSeconds(0.05f);

			if (letter != ' ')
			{
				source.pitch = 1 + Random.Range(-0.3f, 0.3f);
				source.PlayOneShot(bleep, 0.5f);
			}
		}
	}
}
