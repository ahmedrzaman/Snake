﻿using System;
using Gamecmder.Rendering;

namespace Gamecmder.Screens
{
    /// <summary>
    /// Represents the a screen in the screen manager
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// The type of screen
        /// </summary>
        ScreenType Type { get; }


        /// <summary>
        /// Indicates if the screen is currently visible
        /// </summary>
        bool IsVisible { get; }


        /// <summary>
        /// Initialise the screen
        /// </summary>
        void Initialise();


        /// <summary>
        /// Logic update for items in this screen
        /// </summary>
        /// <param name="elapsed"></param>
        void Update(TimeSpan elapsed);


        /// <summary>
        /// Draw the items in this screens
        /// </summary>
        /// <param name="renderer"></param>
        void Draw(IRenderer renderer);


        /// <summary>
        /// Show this screen
        /// </summary>
        void Show();

        /// <summary>
        /// Hide this screen
        /// </summary>
        void Hide();
    }
}
