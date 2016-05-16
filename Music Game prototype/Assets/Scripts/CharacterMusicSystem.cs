using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class CharacterMusicSystem : MonoBehaviour, Quanter {

	public AudioSource drums;
	public AudioSource bass;
	public AudioSource pulse;
	public AudioSource bassSwap;
	public AudioSource pulseSwap;
	public AudioSource teleport;
	public AudioSource dash;
	public AudioSource unfiltered;
	private AudioMixer mixer;
	private BeatMatcher beat;
	private bool loopedBeat;
	private bool loopedQuant;
	private bool loopedActualQuant;
	private AudioClip[] melodies;
	private List<AudioClip[]> notes;
	private AudioClip[] notes1;
	private AudioClip[] notes2;
	private AudioClip[] notes3;
	private AudioClip[] arpeggios;
	public Note activeNote;
	public Note nextNote;
	private Dictionary<Note, AudioClip[]> pulses;
	private Dictionary<Note, AudioClip[]> noteToMelody;
	private Dictionary<Note, AudioClip[]> noteToDashMelody;
	private Dictionary<Note, AudioClip[]> teleports;
	private AudioClip[] dashes;
	public Animator animator;
	private int rayMask = 0;
	private int level = 0;
	private bool clipChange = false;
	private bool swapped = false;
	private AudioClip bubbleCatch;
	private AudioClip bubbleRelease;
	private bool bubbleMute = false;
	
	private void Awake() {
		mixer = bass.outputAudioMixerGroup.audioMixer;
		beat = FindObjectOfType<BeatMatcher> ();
		melodies = Resources.LoadAll<AudioClip>("Audio/Final/Teleports");
		notes1 = Resources.LoadAll<AudioClip>("Audio/Final/Layer 1");
		notes2 = Resources.LoadAll<AudioClip>("Audio/Final/Layer 2");
		notes3 = Resources.LoadAll<AudioClip>("Audio/Final/Layer 3");
		notes = new List<AudioClip[]> ();
		notes.Add (notes1);
		notes.Add (notes2);
		notes.Add (notes3);
		rayMask |= 1 << LayerMask.NameToLayer ("Platform");
		teleports = new Dictionary<Note, AudioClip[]> ();
		teleports.Add(Note.i, Resources.LoadAll<AudioClip>("Audio/Final/Teleports/i"));
		teleports.Add(Note.III, Resources.LoadAll<AudioClip>("Audio/Final/Teleports/III"));
		teleports.Add(Note.iv, Resources.LoadAll<AudioClip>("Audio/Final/Teleports/iv"));
		teleports.Add(Note.v, Resources.LoadAll<AudioClip>("Audio/Final/Teleports/v"));
		teleports.Add(Note.VI, Resources.LoadAll<AudioClip>("Audio/Final/Teleports/VI"));
		teleports.Add(Note.VII, Resources.LoadAll<AudioClip>("Audio/Final/Teleports/VII"));
		pulses = new Dictionary<Note, AudioClip[]> ();
		pulses.Add (Note.i, Resources.LoadAll<AudioClip>("Audio/Final/Pulse/C"));
		pulses.Add (Note.III, Resources.LoadAll<AudioClip>("Audio/Final/Pulse/Eb"));
		pulses.Add (Note.iv, Resources.LoadAll<AudioClip>("Audio/Final/Pulse/F"));
		pulses.Add (Note.v, Resources.LoadAll<AudioClip>("Audio/Final/Pulse/G"));
		pulses.Add (Note.VI, Resources.LoadAll<AudioClip>("Audio/Final/Pulse/Ab"));
		pulses.Add (Note.VII, Resources.LoadAll<AudioClip>("Audio/Final/Pulse/Bb"));
		dashes = Resources.LoadAll<AudioClip>("Audio/Final/Dashes");
		bubbleCatch = Resources.Load<AudioClip> ("Audio/Final/Bubble_catch");
		bubbleRelease = Resources.Load<AudioClip>("Audio/Final/Bubble_release");
		noteToMelody = new Dictionary<Note, AudioClip[]> ();
		noteToMelody.Add (Note.i, buildMelodyClipArray (melodies, 2, 3, 6, 7, 9));
		noteToMelody.Add (Note.III, buildMelodyClipArray (melodies, 6, 7, 9, 1));
		noteToMelody.Add (Note.iv, buildMelodyClipArray (melodies, 8,0,2,3));
		noteToMelody.Add (Note.v, buildMelodyClipArray (melodies, 9,1,4,5));
		noteToMelody.Add (Note.VI, buildMelodyClipArray (melodies, 0,2,3,6,7));
		noteToMelody.Add (Note.VII, buildMelodyClipArray (melodies, 1,4,5,8));
		noteToDashMelody = new Dictionary<Note, AudioClip[]> ();
		noteToDashMelody.Add (Note.i, buildMelodyClipArray (dashes, 2, 3, 6, 7, 9));
		noteToDashMelody.Add (Note.III, buildMelodyClipArray (dashes, 6, 7, 9, 1));
		noteToDashMelody.Add (Note.iv, buildMelodyClipArray (dashes,8,0,2,3));
		noteToDashMelody.Add (Note.v, buildMelodyClipArray (dashes,9,1,4,5));
		noteToDashMelody.Add (Note.VI, buildMelodyClipArray (dashes,0,2,3,6,7));
		noteToDashMelody.Add (Note.VII, buildMelodyClipArray (dashes,1,4,5,8));
	}
	
	private void Start(){
		activeNote = Note.v;
		setClip (Note.i);
		setClip (Note.i);
	}
	
	private AudioClip[] buildMelodyClipArray(AudioClip[] clips, params int[] indexes){
		AudioClip[] melodyClips = new AudioClip[indexes.Length];
		for (int i = 0; i < indexes.Length; i++) {
			melodyClips[i] = clips[indexes[i]];
		}
		return melodyClips;
	}

	IEnumerator Crossfade(){
		for (int i = 0; i <= 100; i+= 10) {
			if(!swapped){
				mixer.SetFloat("bassvolume", ConvertToDecibels(100 - i));
				mixer.SetFloat("swapvolume", ConvertToDecibels(i));
			} else {
				mixer.SetFloat("bassvolume", ConvertToDecibels(i));
				mixer.SetFloat("swapvolume", ConvertToDecibels(100 - i));
			}
			yield return null;
		}
	}

	// Converts values from 0 to 1 into decibels
	public float ConvertToDecibels(int value){
		if (value == 0) {
			return -80f;
		} else if (value == 100) {
			return 0f;
		} else {
			return Mathf.Log((value/100f))/Mathf.Log(10)*20f;
		}
	}


	private void ChangeClip () {
		activeNote = nextNote;
		if (swapped) {
			bass.clip = bassSwap.clip;
			pulse.clip = pulseSwap.clip;
		} else {
			bassSwap.clip = bass.clip;
			pulseSwap.clip = pulse.clip;
		}
		if (swapped) {
			bassSwap.clip = notes [level] [(int)activeNote];
			pulseSwap.clip = pulses [activeNote] [UnityEngine.Random.Range (0, pulses [activeNote].Length)];
		} else {
			bass.clip = notes [level] [(int)activeNote];
			pulse.clip = pulses [activeNote] [UnityEngine.Random.Range (0, pulses [activeNote].Length)];
		}
		if (swapped) {
			StartCoroutine (Crossfade ());
			StartCoroutine (Crossfade ());
		} else {
			StartCoroutine (Crossfade ());
			StartCoroutine (Crossfade ());
		}

		bass.Play ();
		pulse.Play ();
		bassSwap.Play ();
		pulseSwap.Play ();
		bass.timeSamples = drums.timeSamples;
		pulse.timeSamples = drums.timeSamples % 96000;
		bassSwap.timeSamples = drums.timeSamples;
		pulseSwap.timeSamples = drums.timeSamples % 96000;
		swapped = !swapped;
	}

	public void Act(){
		if (clipChange) {
			ChangeClip ();
			clipChange = false;
		} 
		if (bubbleMute) {
			this.SendMessage("MuteMovement");
			bubbleMute = false;
		}
	}
	
	public void setClip(Note note){
		if (note != activeNote && !clipChange) {
			nextNote = note;
			clipChange = true;
			beat.registerBeatOneOff(this);
		}
	}

	public void BubbleCatch(){
		unfiltered.PlayOneShot (bubbleCatch);
		bubbleMute = true;
		beat.registerBeatOneOff (this);
	}

	public void BubbleRelease(){
		unfiltered.PlayOneShot (bubbleRelease, 0.5f);
	}
	
	private void Update(){
		if ((Mathf.Min(drums.timeSamples % 12000, Mathf.Abs((drums.timeSamples % 12000) - 12000)) < 2000) && !loopedQuant) {
			loopedQuant = true;
			beat.ReportDoubleQuant();
		}
		if (drums.timeSamples % 12000 > 6000 && Mathf.Abs((drums.timeSamples % 12000) - 12000) > 4000) {
			loopedQuant = false;
		}
		
		if ((Mathf.Min(drums.timeSamples % 24000, Mathf.Abs((drums.timeSamples % 24000) - 24000)) < 2000) && !loopedBeat) {
			loopedBeat = true;
			beat.ReportBeat();
			animator.SetTrigger("SyncTrigger");
		}
		if (drums.timeSamples % 24000 > 12000 && Mathf.Abs((drums.timeSamples % 24000) - 24000) > 4000) {
			loopedBeat = false;
		}		
	}
	
	public void TeleportSound(Direction direction){
		Note firstNote = activeNote;
		Note secondNote = CheckNoteFromNextPlatform (direction);
		print("From: " + firstNote + " to: " + secondNote);
	//	teleport.PlayOneShot (noteToMelody [activeNote] [UnityEngine.Random.Range (0, noteToMelody [activeNote].Length)], teleport.volume);
		teleport.PlayOneShot (teleports [firstNote] [(int)secondNote], teleport.volume / 2);
	}

	public Note CheckNoteFromNextPlatform(Direction direction){
		Collider2D hit;
		Vector2 startPoint;
		Vector2 rayDirection;
		if (direction == Direction.UP) {
			startPoint = new Vector2 (transform.position.x, transform.position.y + 8f);
			rayDirection = Vector2.down;
		} else {
			startPoint = new Vector2 (transform.position.x, transform.position.y - 6f);
			rayDirection = Vector2.up;
		}
		hit = Physics2D.OverlapCircle (new Vector2 (transform.position.x, transform.position.y), 1f, rayMask);
		if (hit != null) {
			if (hit.tag == "Platform") {
				return hit.gameObject.GetComponent<PlayOnTouch> ().GetNote();
			}
		}
		return activeNote;
	}

	public void DashSound(){
		dash.PlayOneShot (noteToDashMelody[activeNote] [UnityEngine.Random.Range (0, noteToDashMelody[activeNote].Length)], dash.volume);
	}
	
	public void MoveSounds(float move, bool m_Grounded)
	{
		drums.volume = Mathf.Abs (move / 1.5f);
		bass.volume = Math.Abs (move) + 0.7f;
		pulse.volume = Math.Abs (move) + 0.2f;
		mixer.SetFloat ("lowpass",(Mathf.Abs (move) * 19500f) + 2500f);
	}
	
	public void Mute(){
		mixer.SetFloat ("Mute",-80f);
	}
	
	public void UnMute(){
		mixer.SetFloat ("Mute",0f);
	}

	public void LevelUp(){
		level++;
		if (level > 2) {
			level = 2;
		}
	}

	public void HighPass(float amount){
		mixer.SetFloat ("highpass", amount);
	}
}
