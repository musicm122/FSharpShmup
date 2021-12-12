namespace Ello.Singleton

open System
open Godot
open System.Collections.Generic

type AudioManagerFs() =
   inherit Node()

   let bus = "Master"

   let playerCount = 8

   static let instance = new AudioManagerFs()

   static member Instance = instance

   member val AvailablePlayers = List<AudioStreamPlayer>() with get, set

   member val SoundQueue = new Queue<string>() with get, set

   member public this.OnStreamFinished(stream) =
       GD.Print("OnStreamFinished called")
       this.AvailablePlayers.Add(stream)

   member this.printConnections(player: AudioStreamPlayer) : unit =
        let hasSignal = player.HasSignal("finished")
        GD.Print("player.HasSignal('finished') = " +  hasSignal.ToString())
        GD.Print("---------- player connections : ----------")
        let cons = player.GetSignalConnectionList("finshed")
        for item in cons do
            GD.Print(item)
            GD.Print(item.ToString())
        GD.Print("---------- end of player connections : ----------")

   member this.printSignals(player: AudioStreamPlayer) : unit =
        GD.Print("---------- player signals : ----------")
        let signals = player.GetSignalList()
        for item in signals do
            GD.Print(item)
            GD.Print(item.ToString())
        GD.Print("---------- end of player signals : ----------")

   member private this.InitPlayer():Option<AudioStreamPlayer>=
       let player = new AudioStreamPlayer()
       let playerArg = new Collections.Array(player)
       let result = player.Connect("finished", this, "OnStreamFinished", playerArg)
       let isConnected = player.IsConnected("finished",this,"OnStreamFinished")
       GD.Print("Player Is Connected = " + isConnected.ToString())
       //this.printSignals(player)
       //this.printConnections(player)

       match result with
       | Error.Ok ->
           player.Bus <- bus
           Some(player)
       | _ ->
           GD.Print("Could not create AudioPlayer")
           GD.Print(result.ToString())
           None

   member private this.GetAvailablePlayers() =
       [|0..playerCount|]
       |> Array.map (fun(i) ->
           match this.InitPlayer() with
           | result when result.IsSome -> result.Value
           | _ -> null)

   member this.Play(soundPath) =
       //GD.Print("Adding "+soundPath+ " to Queue")
       this.SoundQueue.Enqueue(soundPath)
       //GD.Print("Peek: "+this.SoundQueue.Peek()+ " in Queue")
       //GD.Print("SoundQueue.Count = " + this.SoundQueue.Count.ToString() )

   member this.CheckForAudio() =
        match (this.SoundQueue, this.AvailablePlayers) with
        | (queue, player) when queue.Count > 0 && player.Count > 0 ->
        let dequeuedItem = queue.Dequeue()
        let streamToPlay = ResourceLoader.Load(dequeuedItem):?> AudioStream
        if isNull(streamToPlay) then GD.Print("Stream is null for path " + dequeuedItem )
        try
            player.Item(0).Stream <- streamToPlay
            player.Item(0).Play()
            player.RemoveAt(0)
        with
        | :? Exception as ex -> GD.Print($"{ex.Message}")
        GD.Print("player Count is " + player.Count.ToString())
        | _ ->
            ignore()

   override this._Ready() =
       this.Name <- "AudioManagerFs"
       this.AvailablePlayers.Clear()
       this.GetAvailablePlayers() |> Array.iter(fun(player)-> this.AvailablePlayers.Add(player))

   override this._Process(delta) =
        this.CheckForAudio()