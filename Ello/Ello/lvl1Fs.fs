namespace Ello

open Godot

type lvl1Fs() =
    inherit Node()

    member val SongAudio:AudioStreamPlayer = null with get,set
    
    override this._Ready() =
        this.SongAudio <-
            this.GetNode<AudioStreamPlayer>(new NodePath("Music"))
