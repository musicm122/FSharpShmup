namespace Ello

type MoveDirection =
    | Left
    | Right
    | Up
    | Down

module InputAction =
    [<Literal>]
    let Shoot = "shoot"

    [<Literal>]
    let Left = "left"

    [<Literal>]
    let Right = "right"

    [<Literal>]
    let Up = "up"

    [<Literal>]
    let Down = "down"

module Constants =

    [<Literal>]
    let BulletPath = "res://Bullet.tscn"

    [<Literal>]
    let PlayerPath = "res://Player.tscn"

    [<Literal>]
    let EnemyPath = "res://Enemy.tscn"

module MoveDirectionUtils =
    open Godot

    let MoveDirToVector (dir: MoveDirection) =
        match dir with
        | MoveDirection.Down -> Vector2.Down
        | MoveDirection.Up -> Vector2.Up
        | MoveDirection.Left -> Vector2.Left
        | MoveDirection.Right -> Vector2.Right

module GDUtils =
    open Godot

    let loadTexture name =
        GD.Load<Texture>("res://assets/" + name + ".png")

    let setTexture name (sprite: Sprite) = sprite.Texture <- loadTexture name

    let loadScene name =
        GD.Load<PackedScene>("res://scenes/" + name + ".tscn")

    type Node2D with
        member this.PrintLocalPosition() =
            GD.Print("Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition() =
            GD.Print("Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.PrintLocalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

    type Node with
        member this.ReloadScene() = this.GetTree().ReloadCurrentScene()

        member this.LoadScene<'a when 'a :> Node> scene =
            let scene = loadScene scene
            let node = scene.Instance() :?> 'a
            this.AddChild node
            node

        member this.GetNode<'a when 'a :> Node and 'a: not struct>(path: string) =
            lazy (this.GetNode<'a>(new NodePath(path)))

        member this.addSprite name location =
            let sprite = new Sprite()
            setTexture name sprite
            sprite.Position <- location
            this.AddChild sprite

            sprite

        member this.getChildren() = this.GetChildren() |> Seq.cast<Node>
