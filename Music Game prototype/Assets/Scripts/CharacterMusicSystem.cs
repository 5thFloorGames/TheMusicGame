using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

	public class CharacterMusicSystem : MonoBehaviour
	{
		public AudioSource drums;
		public AudioSource bass;
		public AudioSource pulse;
		public AudioSource bassSwap;
		public AudioSource pulseSwap;
		public AudioSource teleport;
		private AudioMixer mixer;
		private BeatMatcher beat;
		private bool loopedBeat;
		private bool loopedQuant;
		private bool loopedActualQuant;
		private AudioClip[] melodies;
		private AudioClip[] notes;
		private AudioClip[] pulses;
		private AudioClip[] arpeggios;
		public Note activeNote;
		private Dictionary<Note, AudioClip[]> noteToMelody;
		private Dictionary<Note, AudioClip[]> noteToDashMelody;
		private Dictionary<Note, AudioClip[]> shootingSequence;
		private AudioClip[] dashes;
		
		private void Awake()
		{
			mixer = bass.outputAudioMixerGroup.audioMixer;
			beat = FindObjectOfType<BeatMatcher> ();
			melodies = Resources.LoadAll<AudioClip>("Audio/Teleports");
			notes = Resources.LoadAll<AudioClip>("Audio/PlatformNotes/Bass");
			pulses = Resources.LoadAll<AudioClip>("Audio/PlatformNotes/Pulse");
			dashes = Resources.LoadAll<AudioClip>("Audio/Dashes");
			arpeggios = Resources.LoadAll<AudioClip> ("Audio/Shooting/Arpeggios");
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
			shootingSequence = new Dictionary<Note, AudioClip[]> ();
			shootingSequence.Add (Note.i, buildMelodyClipArray (arpeggios, 2, 5,6));
			shootingSequence.Add (Note.III, buildMelodyClipArray (arpeggios, 4, 6,1));
			shootingSequence.Add (Note.iv, buildMelodyClipArray (arpeggios, 2,5,0));
			shootingSequence.Add (Note.v, buildMelodyClipArray (arpeggios, 3,6,1));
			shootingSequence.Add (Note.VI, buildMelodyClipArray (arpeggios, 2,4,0));
			shootingSequence.Add (Note.VII, buildMelodyClipArray (arpeggios, 3, 5, 1));	
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
		
		
		// 0.0625 seconds in length
		IEnumerator fadeIn(AudioMixer source){
			float volume = -20f;
			while (volume < 0f) {
				volume += 1f;
				source.SetFloat("bassvolume", volume);
				yield return null;
			}
		}
		
		IEnumerator fadeOut(AudioMixer mixer, AudioSource source){
			source.mute = false;
			float volume = 0f;
			while (volume > -20f) {
				volume -= 1f;
				mixer.SetFloat("swapvolume", volume);
				yield return null;
			}
			source.mute = true;
		}
		
		public void setClip(Note note){
			if (note != activeNote) {
				activeNote = note;
				bassSwap.clip = bass.clip;
				pulseSwap.clip = pulse.clip;
				StartCoroutine (fadeOut (bassSwap.outputAudioMixerGroup.audioMixer, bassSwap));
				StartCoroutine (fadeOut (pulseSwap.outputAudioMixerGroup.audioMixer, pulseSwap));
				bass.clip = notes [(int)note];
				pulse.clip = pulses [(int)note];
				StartCoroutine (fadeIn (bass.outputAudioMixerGroup.audioMixer));
				StartCoroutine (fadeIn (pulse.outputAudioMixerGroup.audioMixer));
				bass.Play ();
				pulse.Play ();
				bassSwap.Play ();
				pulseSwap.Play ();
				bass.timeSamples = drums.timeSamples;
				pulse.timeSamples = drums.timeSamples;
				bassSwap.timeSamples = drums.timeSamples;
				pulseSwap.timeSamples = drums.timeSamples;
			}
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
			}
			if (drums.timeSamples % 24000 > 12000 && Mathf.Abs((drums.timeSamples % 24000) - 24000) > 4000) {
				loopedBeat = false;
			}			
		}
		
		public void TeleportSound(){
			teleport.PlayOneShot (noteToMelody [activeNote] [UnityEngine.Random.Range (0, noteToMelody [activeNote].Length)], teleport.volume);
		}
		
		public void DashSound(){
			teleport.PlayOneShot (noteToDashMelody[activeNote] [UnityEngine.Random.Range (0, noteToDashMelody[activeNote].Length)], teleport.volume);
		}
		
		
		public void MoveSounds(float move, bool m_Grounded)
		{
			if (!m_Grounded) {
				mixer.SetFloat ("highpass", 1000f); // Placeholder, sounds cool though
			} else {
				mixer.SetFloat ("highpass", 0f);
			}
			drums.volume = Mathf.Abs (move / 4);
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
	}
