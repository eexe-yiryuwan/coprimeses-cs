using Coprimeses.Ops;
//
//
//
//
//
//
/**<summary>  Things related to Coprime Pairs.  </summary>**/
//
namespace Coprimeses {
	//
	//
	//
	//
	//
	//
	/**<summary>  Symbols representing operations. </summary>**/
	//
	namespace Ops {
		using OpType = byte;
		public enum Op: OpType {
			Swi = 0,
			Swa = 1,
			Eul = 2,
		}
		public static class ExplainOp {
			public static String Explain(Op op) {
				return op switch {
					Op.Swi => "Op.Swi // (m,n)->(2*m+n,m)",
					Op.Swa => "Op.Swa // (m,n)->(2*m-n,m)",
					Op.Eul => "Op.Eul // (m,n)->(m+2n,n)",
					_ => $"Unrecognized({(OpType)op}) // it is an error btw...",
				};
			}
		}
		public static class CategorizeOp {
			public static readonly Op[] All = [Op.Swi, Op.Swa, Op.Eul];
		}
	}
	//
	//
	//
	//
	//
	//
	/**<summary>  Symbols representing initial conditions for derivation. </summary>**/
	//
	namespace InitialVariants {
		using InitialVariantType = byte;
		public enum InitialVariant: InitialVariantType {
			Even_Odd = 0,
			Odd_Even = Even_Odd,
			Odd_Odd = 1,
		}
	}
	//
	//
	//
	//
	//
	//
	/**<summary>  Finders of coprimes matching certain criteria.  </summary>**/
	//
	namespace CoprimeFinder {
		enum CoprimeFinderVariant {
			M, N
		}
		public class CoprimeFinder {
			public static Coprimes? FindMBetween(InitialVariants.InitialVariant iv, System.UInt128 lo, System.UInt128 u){
				return FindBetween(CoprimeFinderVariant.M, iv, lo, u);
			}
			public static Coprimes? FindNBetween(InitialVariants.InitialVariant iv, System.UInt128 lo, System.UInt128 u){
				return FindBetween(CoprimeFinderVariant.N, iv, lo, u);
			}
			static Coprimes? FindBetween(CoprimeFinderVariant av, InitialVariants.InitialVariant iv, System.UInt128 lo, System.UInt128 u){
				// init stuff
				System.Random rng = System.Random.Shared;
				Coprimes acc = new(iv);
				System.Collections.Generic.Stack<IEnumerator<Op>> task_options_tree_stack = new();
				// find the value inbetween by applying and reverting operations
				bool converged;
				do {
					// we take actions only when something is out of place
					// so assume at first that the value is in place
					converged = true;
					// take actions if the value falls out of limits
					if((av == CoprimeFinderVariant.M && acc.M < lo) || (av == CoprimeFinderVariant.N && acc.N < lo)) {
						// the number is too small, gotta progress with another operation
						//
						// create next operation plan by shuffling possible things to do
						Op[] options1 = (Op[])Ops.CategorizeOp.All.Clone();
						rng.Shuffle(options1);
						IEnumerator<Op> options2 = options1.AsEnumerable<Op>().GetEnumerator();
						// do the first task in the plan
						options2.MoveNext();
						acc.Do(options2.Current);
						// save the plan on the task stack
						task_options_tree_stack.Push(options2);
						//
						converged = false;
					}
					if((av == CoprimeFinderVariant.M && acc.M > u) || (av == CoprimeFinderVariant.N && acc.N > u)) {
						// the number is too big, revert the last task and try the next from the list
						//
						// get the last/top plan from the stack
						IEnumerator<Op> options1 = task_options_tree_stack.Pop();
						// revert the current task, the same one that was done last time
						acc.Revert(options1.Current);
						// try moving on to the next task
						bool not_the_end_of_options = options1.MoveNext();
						if(not_the_end_of_options){
							// there are more things to try, so try the next
							//
							// apply the next option
							acc.Do(options1.Current);
							// restore the plan to the stack since it is still viable
							task_options_tree_stack.Push(options1);
						} else {
							// no more options in this joint, so go and discard this
							// used up set. BUT... being in this branch means that
							// none of the options satisfy the required PARENT item,
							// so we need to move in the parent domain. None of the
							// already existent branches match current's conditions,
							// so we need a local end-of-options recursive reversal
							// as we can be at multiple-level parent's end-of-options.
							bool parent_popped = task_options_tree_stack.TryPop(out IEnumerator<Op>? maybe_parent);
							while (parent_popped) {
								IEnumerator<Op> parent = maybe_parent!;
								// revert parent action
								acc.Revert(parent.Current);
								// try going to the next parent action
								bool is_there_more = parent.MoveNext();
								if(is_there_more) {
									// so the parent has more to offer, so try it's next
									// branch.
									//
									// apply the next parent action
									acc.Do(parent.Current);
									// restore the parent to the stack
									task_options_tree_stack.Push(parent);
									// cool, try the usual algorithm now
									break;
								} else {
									// nope, this parent is also out of ideas
									// try the further parent. Don't push him
									// back, try popping the next
									//
									// pop the next candidate parent
									parent_popped = task_options_tree_stack.TryPop(out maybe_parent);
									// try it
									continue;
								}
							}
						}
						//
						converged = false;
					}
					if(converged){
						// the value is neither too big nor too small, so the loop can end
						break;
					}
				}while(task_options_tree_stack.Count > 0);
				//
				return (converged)?acc:null;
			}
		}
	}
	//
	//
	//
	//
	/**<summary>  The coprime type.  </summary>**/
	//
	public class Coprimes {
		System.UInt128 m;
		public System.UInt128 M {get => this.m;}
		System.UInt128 n;
		public System.UInt128 N {get => this.n;}
		public Coprimes(InitialVariants.InitialVariant initialvariant) {
			switch(initialvariant) {
				default:
				case InitialVariants.InitialVariant.Even_Odd:
					this.m = 2;
					this.n = 1;
					break;
				case InitialVariants.InitialVariant.Odd_Odd:
					this.m = 3;
					this.n = 1;
					break;
			}
		}
		public override String ToString() {
			System.Text.StringBuilder repr = new("Coprime{m=");
			repr.Append(this.m.ToString());
			repr.Append(",n=");
			repr.Append(this.n.ToString());
			repr.Append('}');
			return repr.ToString();
		}
		public void Do(Ops.Op op) {
			System.UInt128 m2;
			System.UInt128 n2;
			switch(op) {
				default:
				case Ops.Op.Swi:
					m2 = 2 * this.m + this.n;
					n2 = this.m;
					break;
				case Ops.Op.Swa:
					m2 = 2 * this.m - this.n;
					n2 = this.m;
					break;
				case Ops.Op.Eul:
					m2 = this.m + 2 * this.n;
					n2 = this.n;
					break;
			}
			this.m = m2;
			this.n = n2;
		}
		public void Revert(Ops.Op op) {
			System.UInt128 m2;
			System.UInt128 n2;
			switch(op) {
				default:
				case Ops.Op.Swi:
					m2 = this.n;
					n2 = m - 2 * this.n;
					break;
				case Ops.Op.Swa:
					m2 = this.n;
					n2 = 2 * this.n - this.m;
					break;
				case Ops.Op.Eul:
					m2 = this.m - 2 * this.n;
					n2 = this.m;
					break;
			}
			this.m = m2;
			this.n = n2;
		}
	}
	
}