using Reactive.Bindings;
using StrawhatNet.NFC.FelicaReader.Suica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using Kassyi.NFC.Kelmis.Strings;

namespace Kassyi.NFC.Kelmis.Models
{
    //State系クラスを閉じ込めるため、自動生成したファイルと切り分ける
    partial class ScanSFCardPageModel
    {

        /*----------------------------*
         * Event
         *----------------------------*/
        class InitializeEvent { }
        class CardAddedEvent
        {
            public CardAddedEventArgs Args { get; private set; }
            public CardAddedEvent(CardAddedEventArgs args)
            {
                Args = args;
            }
        }
        class CardRemovedEvent
        {
            public CardRemovedEventArgs Args { get; private set; }
            public CardRemovedEvent(CardRemovedEventArgs args)
            {
                Args = args;
            }
        }

        partial class State
        {
            partial void Trace(string st, string ev)
            {
                System.Diagnostics.Debug.WriteLine($"st:{st}, ev:{ev}");
            }
        }

        /*----------------------------*
         * State Actors。Stateの実装。遷移先や、遷移した先でやることを実装する
         * 初回時のEnter()は呼ばれないので、context側で呼び出すこと。
         * またActorには一切プロパティやメンバ変数を持たせないこと。staticクラスと同様の扱いを受けるので混乱のもと。
         *----------------------------*/

#pragma warning disable CS1998
        class BeforeInitializeActor : IScanSFCardActor
        {
            public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct)
            {
                var result = await ct.InitializeAsync();
                if (result.Success)
                    return State.WaitingForCardState;

                ct.StatusMessage.Value = result.Message;
                return State.BeforeInitializeState;
            }
            public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct) => null;
            public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct) => null;

            public void Enter(ScanSFCardPageModel ct)
            {
                ct.Status.Value = ScaningStatus.Disabled;
                ct.StatusMessage.Value = Resources.Disabled;
            }

            public void Exit(ScanSFCardPageModel ct) { }
        }
        class WaitingForCardActor : IScanSFCardActor
        {
            public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct) => null;
            public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct)
            {
                ct.StatusMessage.Value = Resources.Reading;
                var resultRead = await ct.ReadSuicayAsync(ev.Args);
                if (resultRead.Success)
                {
                    ct.PlaySuccessSe();
                    ct.StatusMessage.Value = Resources.TouchIcCard;
                    return State.WaitingForReleaseCardState;
                }
                else
                {
                    var resultInitialize = await ct.InitializeAsync();
                    if (resultInitialize.Success)
                    {
                        ct.PlayErrorSe();
                        ct.StatusMessage.Value = Resources.TouchIcCardAgain;
                        return State.WaitingForAgainTouchState;
                    }
                    return State.BeforeInitializeState;
                }
            }
            public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct) => null;

            public void Enter(ScanSFCardPageModel ct)
            {
                ct.Status.Value = ScaningStatus.Scaning;
                ct.StatusMessage.Value = Resources.TouchIcCard;
            }

            public void Exit(ScanSFCardPageModel ct) { }
        }
        class WaitingForReleaseCardActor : IScanSFCardActor
        {
            public ScaningStatus Status { get; }
            public void Enter(ScanSFCardPageModel ct)
            {
                ct.StatusMessage.Value = Resources.ReleaseCard;
            }
            public void Exit(ScanSFCardPageModel ct) { }
            public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct) => null;
            public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct) => null;
            public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct) => State.WaitingForCardState;
        }
        class WaitingForAgainTouchActor : IScanSFCardActor
        {
            public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct) => null;
            public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct)
            {
                throw new NotImplementedException();
            }
            public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct) => State.WaitingForCardState;

            public void Enter(ScanSFCardPageModel ct)
            {
                ct.Status.Value = ScaningStatus.Error;
            }

            public void Exit(ScanSFCardPageModel ct) { }
        }
    }
}
#pragma warning restore CS1998