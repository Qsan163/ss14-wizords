using System.Linq;
using Content.Server.Administration.Logs;
using Content.Server.Popups;
using Content.Shared.UserInterface;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Interaction;
using Content.Shared.Paper;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Audio.Systems;
using Content.Shared.Popups;
using static Content.Shared.Paper.SharedPaperComponent;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Content.Server.Paper
{
    public sealed class EverythingStampSystem : EntitySystem
    {
        [Dependency] private readonly IAdminLogManager _adminLogger = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly SharedInteractionSystem _interaction = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
        [Dependency] private readonly MetaDataSystem _metaSystem = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;

        [Dependency] private readonly IEntityManager _entities = default!;
        [Dependency] protected readonly SharedPopupSystem Popup = default!;
        public override void Initialize()
        {
            base.Initialize();

            // SubscribeLocalEvent<EverythingStampComponent, ComponentInit>(OnInit);
            // SubscribeLocalEvent<PaperComponent, BeforeActivatableUIOpenEvent>(BeforeUIOpen);
            // SubscribeLocalEvent<PaperComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<EverythingStampComponent, InteractUsingEvent>(OnInteractUsing);
            SubscribeLocalEvent<EverythingStampComponent, UseInHandEvent>(OnInHandActivation);

            // SubscribeLocalEvent<PaperComponent, PaperInputTextMessage>(OnInputTextMessage);

            // SubscribeLocalEvent<ActivateOnPaperOpenedComponent, PaperWriteEvent>(OnPaperWrite);

            // SubscribeLocalEvent<PaperComponent, MapInitEvent>(OnMapInit);
        }
        private void OnInHandActivation(Entity<EverythingStampComponent> entity, ref UseInHandEvent args)
        {
            if (entity.Comp.CollectedStamps.Count >= 2)
            {
                string switchMessageId;
                int positionOfPreviousMode = -1;
                int i = 0;
                foreach (var t in entity.Comp.CollectedStamps)
                {
                    if (t.StampedName == entity.Comp.CurrentStampName)
                    {
                        positionOfPreviousMode = i;
                        break;
                    }
                    i = i + 1;
                }
                if (positionOfPreviousMode == entity.Comp.CollectedStamps.Count-1) {
                    positionOfPreviousMode = -1;
                }
                entity.Comp.CurrentStampName = entity.Comp.CollectedStamps[positionOfPreviousMode + 1].StampedName;
                Popup.PopupEntity(Loc.GetString(entity.Comp.CollectedStamps[positionOfPreviousMode + 1].StampedName), entity, args.User);
                if (TryComp(entity, out StampComponent? stamp)) {
                    stamp.StampedName = entity.Comp.CollectedStamps[positionOfPreviousMode + 1].StampedName;
                }
            }
            else
            {
                Popup.PopupEntity("недостаточно печатей для переключения", entity, args.User);
            }
        }

        private void OnInteractUsing(EntityUid uid, EverythingStampComponent EverythingStampComponent, InteractUsingEvent args)
        {
            var stampComp = _entities.GetComponent<StampComponent>(args.Used);
            if (TryCopyStamp(uid, GetStampInfo(stampComp), stampComp.StampState, EverythingStampComponent)) 
            {
                _audio.PlayPvs(stampComp.Sound, uid);   
                Popup.PopupEntity("печать добавлена", uid, args.User);
            } else {
                Popup.PopupEntity("печать уже есть", uid, args.User);
            }
        }

        private static StampDisplayInfo GetStampInfo(StampComponent stamp)
        {
            return new StampDisplayInfo
            {
                StampedName = stamp.StampedName,
                StampedColor = stamp.StampedColor
            };
        }

        public bool TryCopyStamp(EntityUid uid, StampDisplayInfo stampInfo, string spriteStampState, EverythingStampComponent EverythingStampComponent)
        {
            bool ifAlreadyInCollected = true;
            foreach (var t in EverythingStampComponent.CollectedStamps)
            {
                if (stampInfo.Equals(t)) {
                    ifAlreadyInCollected = true;
                }
            }
            if (ifAlreadyInCollected) {
                EverythingStampComponent.CollectedStamps.Add(stampInfo);
            }
            return ifAlreadyInCollected;
        }
    }
}