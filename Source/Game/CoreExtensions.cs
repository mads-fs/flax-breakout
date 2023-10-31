using FlaxEngine;
using System;

namespace Game.Extensions
{
    /// <summary>This class provides extensions for FlaxEngines core Api Calls.</summary>
    public static class CoreExtensions
    {
        public static Actor Instantiate(Prefab prefab)
            => PrefabManager.SpawnPrefab(prefab);
        public static Actor Instantiate(Prefab prefab, Actor parent)
            => PrefabManager.SpawnPrefab(prefab, parent);
        public static Actor Instantiate(Prefab prefab, Transform transform)
            => PrefabManager.SpawnPrefab(prefab, transform);
        public static Actor Instantiate(Prefab prefab, Vector3 position)
            => PrefabManager.SpawnPrefab(prefab, position);
        public static Actor Instantiate(Prefab prefab, Actor parent, Vector3 position, Quaternion rotation)
            => PrefabManager.SpawnPrefab(prefab, position, rotation);
        public static Actor Instantiate(Prefab prefab, Actor parent, Vector3 position, Quaternion rotation, Vector3 scale)
            => PrefabManager.SpawnPrefab(prefab, position, rotation, scale);

        /// <summary>
        /// Will find the first instance of the given type that is either an <see cref="Actor"/> or a <see cref="Script"/>.
        /// </summary>
        /// <typeparam name="T">The type that is being looked for.</typeparam>
        /// <param name="actor">The actor that this method is called on.</param>
        /// <returns>The specified type or null</returns>
        public static T Get<T>(this Actor actor) where T : SceneObject
        {
            if (actor == null) return null;
            if (actor.ScriptsCount > 0 && typeof(T).IsSubclassOf(typeof(Script)))
            {
                return actor.GetScript<T>();
            }
            if (typeof(T).IsSubclassOf(typeof(Actor)))
            {
                return actor as T;
            }
            return null;
        }

        /// <summary>
        /// Will find the first instance of the given type that is either an <see cref="Actor"/> or a <see cref="Script"/> in the Actor's children.
        /// </summary>
        /// <typeparam name="T">The type that is being looked for.</typeparam>
        /// <param name="actor">The actor that this method is called on.</param>
        /// <param name="includeSelf">Whether to include the given Actor object in the search or not.</param>
        /// <returns>The specified type or null.</returns>
        public static T GetInChildren<T>(this Actor actor, bool includeSelf = false) where T : SceneObject
        {
            if (actor == null) return null;
            if (actor.ChildrenCount == 0 && includeSelf == false) return null;
            if (actor.ChildrenCount == 0 && includeSelf == true) return actor.Get<T>();

            if (typeof(T).IsSubclassOf(typeof(Actor)))
            {
                foreach (Actor child in actor.GetChildren<Actor>())
                {
                    if (child is T) return child as T;
                }
                return null;
            }
            if (typeof(T).IsSubclassOf(typeof(Script)))
            {
                foreach (Actor child in actor.GetChildren<Actor>())
                {
                    if (child.ScriptsCount > 0)
                    {
                        T target = child.GetScript<T>();
                        if (target) return target;
                    }
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// This extension simplifies getting an <see cref="Actor"/> or a <see cref="Script"/> in the specified Actors parent.
        /// </summary>
        /// <typeparam name="T">The type that is being looked for.</typeparam>
        /// <param name="actor">The actor that this method is called on.</param>
        /// <param name="includeSelf">Whether to include the given Actor object in the search or not.</param>
        /// <param name="recursive">Set this to true if the search should recursively go through every parent of the Actors parent.</param>
        /// <returns>The specified type or null.</returns>
        public static T GetInParent<T>(this Actor actor, bool includeSelf = false, bool recursive = false) where T : SceneObject
        {
            if (actor == null) return null;
            if (actor.Parent == null) return null;

            if (recursive)
            {
                Actor parent = actor.Parent;
                do
                {
                    T target = parent.Get<T>();
                    if (target) return target;
                    parent = parent.Parent;
                } while (parent != null);

                if (includeSelf) return actor.Parent.Get<T>();
                return null;
            }
            if (includeSelf)
            {
                T target = actor.Parent.Get<T>();
                if (target) return target;
                else return actor.Get<T>();
            }

            return actor.Parent.Get<T>();
        }

        /// <summary>
        /// Will find all instances of the given type that are <see cref="Script"/>s on the given <see cref="Actor"/>.
        /// </summary>
        /// <typeparam name="T">The type that is being looked for.</typeparam>
        /// <param name="actor">The actor that this method is called on.</param>
        /// <returns><see cref="T[]"/> of the specified type or <see cref="Array.Empty{T}"/></returns>
        public static T[] GetMultiple<T>(this Actor actor) where T : Script
        {
            if (actor == null || actor.ScriptsCount == 0) return Array.Empty<T>();
            return actor.GetScripts<T>();
        }

        public static T[] GetMultipleInChildren<T>(this Actor actor, bool includeSelf) where T : SceneObject
        {
            if (actor == null) return Array.Empty<T>();
            if (actor.Children.Length == 0 && actor.ScriptsCount == 0 && includeSelf == false) return Array.Empty<T>();
            if (actor.Children.Length == 0 && actor.ScriptsCount == 0 && includeSelf == true) return new T[1] { actor as T };

            return Array.Empty<T>();
        }

        public static T[] GetMultipleInParents<T>(this Actor actor, bool includeSelf, bool recursive) where T : SceneObject
        {
            return Array.Empty<T>();
        }
    }
}
