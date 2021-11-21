namespace Ello

type IHealth =
    abstract GetHealth: unit -> int
    abstract TakeDamage: int -> unit
    abstract HealDamage: int -> unit
    abstract Die: unit -> unit

type IShootable = 
    abstract AttackPower: int
