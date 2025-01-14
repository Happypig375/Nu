﻿// Nu Game Engine.
// Copyright (C) Bryan Edds, 2013-2020.

namespace OmniBlade
open System.Numerics
open Prime
open Nu
open Nu.Declarative
open OmniBlade

[<AutoOpen>]
module ReticlesDispatcher =

    type Reticles =
        Map<CharacterIndex, Vector2>

    type ReticlesCommand =
        | TargetCancel
        | TargetSelect of CharacterIndex

    type Entity with
        member this.GetReticles = this.GetModel<Reticles>
        member this.SetReticles = this.SetModel<Reticles>
        member this.Reticles = this.Model<Reticles> ()
        member this.TargetSelectEvent = Events.TargetSelect --> this

    type ReticlesDispatcher () =
        inherit GuiDispatcher<Reticles, unit, ReticlesCommand> (Map.empty)

        static member Properties =
            [define Entity.SwallowMouseLeft false
             define Entity.Visible false]

        override this.Command (_, command, rets, world) =
            match command with
            | TargetCancel -> just (World.publishPlus () rets.CancelEvent [] rets true world)
            | TargetSelect index -> just (World.publishPlus index rets.TargetSelectEvent [] rets true world)

        override this.Content (reticles, rets) =
            [Content.button (rets.Name + "+" + "Cancel")
                [Entity.PositionLocal == Constants.Battle.CancelPosition
                 Entity.Size == v2 48.0f 48.0f
                 Entity.UpImage == asset Assets.Battle.PackageName "CancelUp"
                 Entity.DownImage == asset Assets.Battle.PackageName "CancelDown"
                 Entity.ClickEvent ==> cmd TargetCancel]
             Content.entities reticles constant constant $ fun index center _ ->
                Content.button (rets.Name + "+Reticle+" + CharacterIndex.toEntityName index)
                    [Entity.Size == v2 96.0f 96.0f
                     Entity.Center <== center
                     Entity.UpImage == asset Assets.Battle.PackageName "ReticleUp"
                     Entity.DownImage == asset Assets.Battle.PackageName "ReticleDown"
                     Entity.ClickEvent ==> cmd (TargetSelect index)]]