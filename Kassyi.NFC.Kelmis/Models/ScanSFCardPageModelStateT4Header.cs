//
//T4 Genelated
//
using System;
using System.Threading.Tasks;


namespace Kassyi.NFC.Kelmis.Models
{
    partial class ScanSFCardPageModel{

	interface IScanSFCardActor{
		void Enter(ScanSFCardPageModel ct);
		void Exit(ScanSFCardPageModel ct);
		Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct);
		Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct);
		Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct);
	}

  abstract partial class State {
    public static State BeforeInitializeState { get; } = new BeforeInitialize();
    public static State WaitingForCardState { get; } = new WaitingForCard();
    public static State WaitingForReleaseCardState { get; } = new WaitingForReleaseCard();
    public static State WaitingForAgainTouchState { get; } = new WaitingForAgainTouch();
    public static State Ignore { get;} = null;

    partial void Trace(string st, string ev);
    protected abstract string Name{get;}
	public abstract void Enter(ScanSFCardPageModel ct);
	public abstract void Exit(ScanSFCardPageModel ct);


	protected abstract Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct);
    public async Task<State> Handle(InitializeEvent ev, ScanSFCardPageModel context) { 
		Trace(Name,nameof(InitializeEvent));
		var nextState = await DoAction(ev, context);
		if(nextState ==null)
			nextState = this;
		else{
			Exit(context);
			nextState.Enter(context);	
		}
		return nextState;
    }
	protected abstract Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct);
    public async Task<State> Handle(CardAddedEvent ev, ScanSFCardPageModel context) { 
		Trace(Name,nameof(CardAddedEvent));
		var nextState = await DoAction(ev, context);
		if(nextState ==null)
			nextState = this;
		else{
			Exit(context);
			nextState.Enter(context);	
		}
		return nextState;
    }
	protected abstract Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct);
    public async Task<State> Handle(CardRemovedEvent ev, ScanSFCardPageModel context) { 
		Trace(Name,nameof(CardRemovedEvent));
		var nextState = await DoAction(ev, context);
		if(nextState ==null)
			nextState = this;
		else{
			Exit(context);
			nextState.Enter(context);	
		}
		return nextState;
    }
  }

  class BeforeInitialize : State {

    protected override string Name{ get; } = nameof(BeforeInitialize);
	private static readonly BeforeInitializeActor actor = new BeforeInitializeActor();
	public override void Enter(ScanSFCardPageModel ct)=> actor.Enter(ct);
	public override void Exit(ScanSFCardPageModel ct)=> actor.Exit(ct);

	protected override Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
  }
  class WaitingForCard : State {

    protected override string Name{ get; } = nameof(WaitingForCard);
	private static readonly WaitingForCardActor actor = new WaitingForCardActor();
	public override void Enter(ScanSFCardPageModel ct)=> actor.Enter(ct);
	public override void Exit(ScanSFCardPageModel ct)=> actor.Exit(ct);

	protected override Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
  }
  class WaitingForReleaseCard : State {

    protected override string Name{ get; } = nameof(WaitingForReleaseCard);
	private static readonly WaitingForReleaseCardActor actor = new WaitingForReleaseCardActor();
	public override void Enter(ScanSFCardPageModel ct)=> actor.Enter(ct);
	public override void Exit(ScanSFCardPageModel ct)=> actor.Exit(ct);

	protected override Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
  }
  class WaitingForAgainTouch : State {

    protected override string Name{ get; } = nameof(WaitingForAgainTouch);
	private static readonly WaitingForAgainTouchActor actor = new WaitingForAgainTouchActor();
	public override void Enter(ScanSFCardPageModel ct)=> actor.Enter(ct);
	public override void Exit(ScanSFCardPageModel ct)=> actor.Exit(ct);

	protected override Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
	protected override Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
		return actor.DoAction(ev, ct);
	}
  }

}}/*

  class InitializeEvent {}
  class CardAddedEvent {}
  class CardRemovedEvent {}

  interface ScanSFCardPageModel{}

  partial class State {
    partial void Trace(string st, string ev) {}
  }


class BeforeInitializeActor : IScanSFCardActor{
	public ScaningStatus Status{ get; }
	public override void Enter(ScanSFCardPageModel ct){}
	public override void Exit(ScanSFCardPageModel ct){}
	public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
	}
}
class WaitingForCardActor : IScanSFCardActor{
	public ScaningStatus Status{ get; }
	public override void Enter(ScanSFCardPageModel ct){}
	public override void Exit(ScanSFCardPageModel ct){}
	public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
	}
}
class WaitingForReleaseCardActor : IScanSFCardActor{
	public ScaningStatus Status{ get; }
	public override void Enter(ScanSFCardPageModel ct){}
	public override void Exit(ScanSFCardPageModel ct){}
	public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
	}
}
class WaitingForAgainTouchActor : IScanSFCardActor{
	public ScaningStatus Status{ get; }
	public override void Enter(ScanSFCardPageModel ct){}
	public override void Exit(ScanSFCardPageModel ct){}
	public async Task<State> DoAction(InitializeEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardAddedEvent ev, ScanSFCardPageModel ct){
	}
	public async Task<State> DoAction(CardRemovedEvent ev, ScanSFCardPageModel ct){
	}
}
*/

