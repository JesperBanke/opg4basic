using System;
using System.IO;
using System.Collections.Generic;
using DIKUArcade.EventBus;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
public class Player {//: IGameEventProcessor<object> {
    public Entity Entity {get; private set;}
 
    public Player(DynamicShape shape, IBaseImage image) {
        Entity = new Entity(shape, image);
    }
    public void direction(Vec2F vec) { 
        this.Entity.Shape.AsDynamicShape().Direction = vec;
    }
    public void Move(){
        this.Entity.Shape.Move(this.Entity.Shape.AsDynamicShape().Direction);
        if (Entity.Shape.Position.X < 0 ||
                            Entity.Shape.Position.X + Entity.Shape.Extent.X > 1) {
            this.Entity.Shape.SetPosition(this.Entity.Shape.Position -
                                        this.Entity.Shape.AsDynamicShape().Direction);
        }
    }
    public void shot(){
        PlayerShot shot = new PlayerShot(
            new DynamicShape(new Vec2F(Entity.Shape.Position.X + 
                                       Entity.Shape.Extent.X / 2,
                                       Entity.Shape.Position.Y +
                                       Entity.Shape.Extent.Y - 0.01f), 
                            new Vec2F(0.008f, 0.027f)),
            new Image(Path.Combine("Assets", "Images", "BulletRed2.png")));
        Game.PlayerShots.Add(shot);
    }
}