namespace Ello

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
            GD.Print(msg+" Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition(msg) =
            GD.Print(msg+" Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

    type Node with
        member this.LoadScene<'a when 'a :> Node> scene =
            let scene = loadScene scene
            let node = scene.Instance() :?> 'a
            this.AddChild node
            node

        member this.getNode<'a when 'a :> Node and 'a: not struct>(path: string) =
            lazy (this.GetNode<'a>(new NodePath(path)))

        member this.addSprite name location =
            let sprite = new Sprite()
            setTexture name sprite
            sprite.Position <- location
            this.AddChild sprite

            sprite

        member this.getChildren() = this.GetChildren() |> Seq.cast<Node>
