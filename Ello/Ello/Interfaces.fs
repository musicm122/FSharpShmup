namespace Ello

open Godot

type EntityHealth =
    { mutable MaxHp: float
      mutable CurrentHp: float
      OnHeal: float-> unit
      OnDeath: unit -> unit
      OnDamage: float -> unit
      IsDead: unit -> bool }
    static member Default() =
        let _MaxHp = 2.0
        let _CurrentHp = 2.0

        { MaxHp = _MaxHp
          CurrentHp = _CurrentHp
          IsDead =
            fun () ->
                _CurrentHp <= 0.0
          OnDamage = 
            fun (damageAmt) -> GD.Print("OnDamage called with "+ damageAmt.ToString())
          OnHeal = 
            fun (healAmt) -> GD.Print("OnHeal Called with "+ healAmt.ToString())
          OnDeath = 
            fun () -> GD.Print("OnDeath Called") }

type DestroyableData =
    { TTL: float32
      mutable OnDestroy: unit -> unit }
    static member Default() =
        let _onDestroy = fun () -> GD.Print("OnDestroy")
        { TTL = 5f; OnDestroy = _onDestroy }

type Destroyable(arg: DestroyableData) =
    let mutable _accumulatedTime = 0f
    member val TimeToLive = arg.TTL: float32 with get, set
    member val Destroy = arg.OnDestroy with get, set

    member this.AccumulateTime(delta) =
        _accumulatedTime <- _accumulatedTime + delta

        if _accumulatedTime > this.TimeToLive then
            this.Destroy()

type AmmoData =
    { AttackPower: float
      Speed: float32
      mutable Destroyable: Destroyable }
    static member Default() =
        let _destroyable = Destroyable(DestroyableData.Default())

        { AttackPower = 1.0
          Speed = 100f
          Destroyable = _destroyable }


type Ammo(args: AmmoData) =
    member val OnCollisionFunc = ignore with get, set
    member val AttackPower = args.AttackPower with get, set
    member val Speed = args.Speed with get, set
    member val Destroyable = args.Destroyable

    member this.OnCollision(body: Node) =
        this.OnCollisionFunc(body, this.AttackPower)
        this.Destroyable.Destroy()


type HealthProvider(args: EntityHealth) =    
    member val MaxHp = args.MaxHp with get, set
    member val CurrentHp = args.CurrentHp with get, set
    member val OnDamage = args.OnDamage with get, set
    member val OnHeal = args.OnHeal with get, set
    member val OnDeath = args.OnDeath with get, set    

    member this.ClampHp(currVal) = 
        MathUtils.clampMinZero this.MaxHp currVal
    
    member this.TakeDamage amt =
        this.CurrentHp <- this.ClampHp (this.CurrentHp - amt) 
        this.OnDamage(amt)
        if this.CurrentHp <= 0.0 then this.Die()

    member this.HealDamage amt =
        this.CurrentHp <- this.ClampHp (this.CurrentHp + amt) 
        this.OnHeal(amt)

    member this.Die() : unit = this.OnDeath()
