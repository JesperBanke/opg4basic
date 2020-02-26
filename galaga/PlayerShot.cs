using System;
using System.IO;
using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
public class PlayerShot : Entity{
    public Entity Entity {get; private set;}
    private List<Image> explosionStrides;
    private static AnimationContainer explosions;
    private int explosionLength = 500;
    public PlayerShot(DynamicShape shape, IBaseImage image) : base(shape, image) {
        Entity = new Entity(shape, image);
        Entity.Shape.AsDynamicShape().Direction = new Vec2F(0.0f,0.01f);

        explosionStrides = ImageStride.CreateStrides(8,
            Path.Combine("Assets", "Images", "Explosion.png"));
        explosions = new AnimationContainer(8);
    }

    public static void iterateShots() {
        foreach (var shot in Game.PlayerShots){
            shot.Shape.Move();
            if (shot.Shape.Position.Y > 1.0f) {
                 shot.DeleteEntity();
            }
            else {
                foreach (var enemy in Game.enemies) {
                    var col = CollisionDetection.Aabb(shot.Shape.AsDynamicShape(),enemy.Shape);
                    if(col.Collision){
                        shot.AddExplosion(enemy.Shape.Position.X, enemy.Shape.Position.Y,
                                enemy.Shape.Extent.X, enemy.Shape.Extent.Y);
                        explosions.RenderAnimations();
                        
                        enemy.DeleteEntity();
                        shot.DeleteEntity();
                    }
                }
            }
        }
    }
    public static List<PlayerShot> updateShot(){
        List<PlayerShot> newPlayerShot = new List<PlayerShot>();
        foreach (PlayerShot shot in Game.PlayerShots) {
            if (!shot.IsDeleted()) {
                newPlayerShot.Add(shot);
            }
        }
        return newPlayerShot;
    }
     public void AddExplosion(float posX, float posY, float extentX, float extentY) {
        explosions.AddAnimation(
            new StationaryShape(posX, posY, extentX, extentY), explosionLength,
            new ImageStride(explosionLength / 8, explosionStrides));
        //explosions.RenderAnimations();
    }   

}