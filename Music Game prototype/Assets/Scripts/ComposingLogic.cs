using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComposingLogic : MonoBehaviour {

	private struct NoteArray
	{
		public Note[] notes;

		public NoteArray(params Note[] notes){
			this.notes = notes;
		}
	}
	
	private class NoteNode{

		private NoteArray[] noteArrays;

		public NoteNode(params NoteArray[] notes){
			this.noteArrays = notes;
		}

		public Note[] Get(int level){
			if (level - 1 < 0) {
				return this.noteArrays[0].notes;
			} else {
				return this.noteArrays[level - 1].notes;
			}
		}
	}

	private Dictionary<Note, NoteNode> noteTransitions;
	private int level = 0;
	private Note lastNote = Note.i;
	
	// Use this for initialization
	void Start () {
		noteTransitions = new Dictionary<Note, NoteNode> ();

		noteTransitions.Add(Note.i, new NoteNode(new NoteArray(Note.III, Note.iv, Note.v, Note.VI, Note.VII)));
		noteTransitions.Add(Note.III, new NoteNode(
			new NoteArray(Note.v, Note.VI, Note.VII),
			new NoteArray(Note.v, Note.VII),
			new NoteArray(Note.i)
		));
		noteTransitions.Add(Note.iv, new NoteNode(
			new NoteArray(Note.III, Note.VI, Note.VII),
			new NoteArray(Note.v, Note.VI, Note.VII),
			new NoteArray(Note.i)
		));
		noteTransitions.Add(Note.v, new NoteNode(
			new NoteArray(Note.III, Note.VI, Note.VII),
			new NoteArray(Note.VI, Note.VII),
			new NoteArray(Note.i)
		));
		noteTransitions.Add(Note.VI, new NoteNode(
			new NoteArray(Note.III, Note.iv, Note.VII),
			new NoteArray(Note.III, Note.v, Note.VII),
			new NoteArray(Note.i)
			));
		noteTransitions.Add(Note.VII, new NoteNode(
			new NoteArray(Note.III, Note.iv, Note.VI),
			new NoteArray(Note.III, Note.v, Note.VI),
			new NoteArray(Note.i)
			));
	}

	public Note nextNotes(Note note, int level){
		Note[] notes = noteTransitions [note].Get(level);

		return notes[Random.Range(0, notes.Length)];
	}

	// Update is called once per frame
	void Update () {
		Note newNote = nextNotes (lastNote, level);
		print (newNote);
		lastNote = newNote;
		level++;
		if (level > 3) {
			level = 0;
		}
	}
}
