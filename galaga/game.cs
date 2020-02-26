using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.EventBus;
using DIKUArcade.Math;
using DIKUArcade.Graphics;
using DIKUArcade.Entities;


public class Game : IGameEventProcessor<object> {
    private Window win;
    private DIKUArcade.Timers.GameTimer gameTimer;
    private Player player;
    private GameEventBus<object> eventBus;
    private List<Image> enemyStrides;
    public static List<Enemy> enemies {get; private set;}
    public static List<PlayerShot> PlayerShots {get; private set;}
    //keypress / keyrelease simple fix.
    private bool KEY_A = false;
    private bool KEY_D = false;
    private Score score;

    public Game() {
        win = new Window("galaga", 500, 500);
        gameTimer = new DIKUArcade.Timers.GameTimer(60,60);
        player = new Player(
            new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
            new Image(Path.Combine("Assets", "Images", "Player.png")));

        eventBus = new GameEventBus<object>();
            eventBus.InitializeEventBus(new List<GameEventType>() {
            GameEventType.InputEvent, // key press / key release
            GameEventType.WindowEvent, // messages to the window
            });

        win.RegisterEventBus(eventBus);
        eventBus.Subscribe(GameEventType.InputEvent, this); 
        eventBus.Subscribe(GameEventType.WindowEvent, this);
        

        enemyStrides = ImageStride.CreateStrides(4,
            Path.Combine("Assets", "Images", "BlueMonster.png"));
        enemies = new List<Enemy>();
        Enemy.addEnemies(new ImageStride(80, enemyStrides));

        PlayerShots = new List<PlayerShot>();
        
        score = new Score(new Vec2F(0.9f,0.9f), new Vec2F(0.1f, 0.1f));

    }

    public void GameLoop() {
        while(win.IsRunning()) {
            gameTimer.MeasureTime();
            while(gameTimer.ShouldUpdate()) {
                eventBus.ProcessEvents();
                win.PollEvents();
            }
            if (gameTimer.ShouldRender()) {
                win.Clear();
                player.Entity.RenderEntity();
                score.RenderScore();
                player.Move();
                foreach(var enemy in enemies) {
                    enemy.Entity.RenderEntity();
                }
                PlayerShot.iterateShots();
                foreach(var shot in PlayerShots){
                    shot.Entity.RenderEntity();
                }
                newEnemies();
                PlayerShots = PlayerShot.updateShot();
                win.SwapBuffers();
            }
            if (gameTimer.ShouldReset()) {
                win.Title = "galga | UPS" + gameTimer.CapturedUpdates + 
                            ", FPS " + gameTimer.CapturedUpdates;

            }

        }
    }
    public void KeyPress(string  key){
        switch(key) {
            case "KEY_ESCAPE":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                break;
            case "KEY_A":
                KEY_A = true;
                player.direction(new Vec2F(-0.01f,0.0f));
                break;
            case "KEY_D":
                KEY_D = true;
                player.direction(new Vec2F(0.01f,0.0f));
                break;
            case "KEY_SPACE":
                player.shot();
                break;
        }
    }
    public void KeyRelease(string key){
         switch(key) {
            case "KEY_ESCAPE":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                break;
            case "KEY_A":
                if (KEY_D){
                    KeyPress("KEY_D");
                    KEY_A = false; 
                    break;
                }
                player.direction(new Vec2F(0.0f,0.0f));
                KEY_A = false;
                break;
            case "KEY_D":
                if (KEY_A){
                    KeyPress("KEY_A");
                    KEY_D = false;
                    break;
                }
                player.direction(new Vec2F(0.0f,0.0f));
                KEY_D = false;
                break;
        }
    }
    public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
        if (eventType == GameEventType.WindowEvent) {
            switch (gameEvent.Message) {
                case "CLOSE_WINDOW":
                    win.CloseWindow();
                    break;
                default:
                    break;
            }
        } else if (eventType == GameEventType.InputEvent) {
            switch (gameEvent.Parameter1) {
                case "KEY_PRESS":
                    KeyPress(gameEvent.Message);
                    break;
                case "KEY_RELEASE":
                    KeyRelease(gameEvent.Message);
                    break;
            }
        }
    }
    public void newEnemies(){
    List<Enemy> newEnemies = new List<Enemy>();
        foreach (Enemy enemy in Game.enemies) {
            if (enemy.IsDeleted()) {
                score.AddPoint(10);
            }
            else
                newEnemies.Add(enemy);
        }
        Game.enemies = newEnemies;
    }
} 

