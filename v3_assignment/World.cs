using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace v3_assignment
{
    class World
    {
        public static Dictionary<string, GameEntity> Objects = new Dictionary<string, GameEntity>();
        private static LinkedList<GameEntity> DrawList = new LinkedList<GameEntity>();
        public static bool stoprespawn = false;
        public static void Add(string key, GameEntity obj)
        {
            try
            {
                Objects.Add(key, obj);
                DrawList.AddLast(obj);
                
            }
            catch (Exception)
            {
                // TODO: Show error message when a duplicated key is inserted.
                Console.WriteLine("World.Add: Exception");
            }
        }

        public static void stopall()
        {
            try
            {
                foreach(GameEntity entity in DrawList)
                {
                    entity.Alive = false;
                    entity.Position = entity.Origin;
                    
                }
                stoprespawn = true;
            }
            catch (Exception)
            {
                // TODO: Show error message when a duplicated key is inserted.
                Console.WriteLine("World.Add: Exception");
            }
        }
        public static void setrespawn()
        {
            stoprespawn = false;
        }

        public static bool getspawn()
        {
            return stoprespawn;
        }

        public static void Remove(string key)
        {
            try
            {
                DrawList.Remove(Objects[key]);
                Objects.Remove(key);
            }
            catch (Exception)
            {
                // TODO: Show error message on any errors that may occur.
                Console.WriteLine("World.Add: Exception");
            }
        }

        public static void Clear()
        {
            Objects.Clear();
            DrawList.Clear();
        }

        public static void Debug()
        {
            foreach (KeyValuePair<string, GameEntity> pair in Objects)
                Console.WriteLine(pair.Key);

            foreach (GameEntity obj in DrawList)
                Console.WriteLine(obj.Position);
        }

        public static void Initialize(ContentManager content)
        {
            foreach (GameEntity obj in DrawList)
                obj.Initialize(content);
        }

        public static void Update(GameTime gameTime)
        {
            LinkedListNode<GameEntity> itr = DrawList.First;
            while (itr != null)
            {
                if (itr.Value.Alive)
                    itr.Value.Update(gameTime);
                itr = itr.Next;
            }


        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (GameEntity obj in DrawList)
                if (obj.Alive)
                    obj.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
