namespace Ello

open Godot

type PlayerCameraFs() =
    inherit Camera2D()

    //member val CurrentShake:ScreenShakeInstance = ScreenShakeInstance.Default() with get,set
    member val Duration = 0f with get, set
    member val PeriodInMs = 0f with get, set
    member val Amplitude = 0f with get, set
    member val RemainingDuration = 0f with get, set
    member val LastShookTimer = 0f with get, set
    member val previous_x = 0.0f with get, set
    member val previous_y = 0.0f with get, set
    member val last_offset = Vector2.Zero with get, set

    //member this.Intensity(timer) =this.Amplitude * (1.0f - ((this.Duration - timer) / this.Duration))
    //member this.PeriodInMs() =1.0f / this.Frequency

    member this.ScheduleShake(shake: ScreenShakeInstance): unit =
        GD.Print("scheduling new Shake")
        //this.CurrentShake <- shake
        this.Duration <- shake.Duration
        this.RemainingDuration <- shake.Duration
        this.PeriodInMs <- 1.0f / shake.Frequency
        this.Amplitude <- shake.Amplitude

        this.previous_x <- GDUtils.getRandomInRange -1.0f 1.0f
        this.previous_y <- GDUtils.getRandomInRange -1.0f 1.0f

        this.Offset <- this.Offset - this.last_offset
        this.last_offset <- Vector2.Zero

    override this._Ready() = this.SetProcess(true)

    //wip adapting from https://github.com/MrEliptik/godot_experiments
    override this._Process (delta): unit =

        match this.RemainingDuration = 0f with
        | true -> ignore()
        | false ->
            this.LastShookTimer <- this.LastShookTimer + delta
            while this.LastShookTimer > Mathf.Round(this.PeriodInMs) do
                GD.Print("(this.LastShookTimer) = ", this.LastShookTimer)
                GD.Print("(Mathf.Round(this.PeriodInMs)) = ", Mathf.Round(this.PeriodInMs))
                this.LastShookTimer <- this.LastShookTimer - this.PeriodInMs
                let intensity = this.Amplitude * (1f - ((this.Duration - this.RemainingDuration) / this.Duration))
                let newX = GDUtils.getRandomInRange -1.0f 1f
                let xComponent = intensity * (this.previous_x + (delta * (newX - this.previous_x)))
                let newY = GDUtils.getRandomInRange -1.0f 1f
                let yComponent = intensity * (this.previous_y + (delta * (newY - this.previous_y)))
                this.previous_x <- newX
                this.previous_y <- newY
                let newOffset = new Vector2(xComponent, yComponent)
                this.Offset <- this.Offset - this.last_offset + newOffset
                this.last_offset <- newOffset
            this.RemainingDuration <- this.RemainingDuration - delta
            if this.RemainingDuration <= 0f then
                this.RemainingDuration <- 0f
                this.Offset <- this.Offset - this.last_offset
