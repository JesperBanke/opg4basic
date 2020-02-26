using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using System.Collections.Generic;
public class Enemy : Entity {
    public Entity Entity {get; private set;}
    public Enemy(DynamicShape shape, IBaseImage image) : base(shape, image) {
        Entity = new Entity(shape, image);
    }

    public static void addEnemies(IBaseImage image) {
        for (int i = 1; i < 9; i++){
            Enemy enemy = new Enemy(new DynamicShape(new Vec2F(0.10f * i, 0.90f),
                                    new Vec2F(0.1f, 0.1f)),image);
            Game.enemies.Add(enemy); 
        }
    }
}