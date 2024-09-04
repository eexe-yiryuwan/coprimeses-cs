//
//
//
//
/**<summary>  The program entry and coordination.  </summary>**/
//
namespace Main {
	//
	//
	//
	//
	/**<summary>///
		<para>
			Main function uses these, to remember internaly,
			what action to take in the end after finishing
			it with cli args.
		</para>
		<para>
			Useful to flatten program structure: to
			transfer final action between interpreting
			cli args and actually doing anything concrete.
		</para>
	///</summary>**/
	//
	enum Action{
		UNRECOGNIZED,
		FIND_M_BETWEEN, FIND_N_BETWEEN
	}
	//
	//
	//
	//
	//
	//
	/**<summary>  The Main Program Entry Boi (+ help text)  </summary>**/
	//
	static class App {
		static readonly System.String hepl_text = """
			usage: coprime <options>

			coprime description: Coprime(m,n)  ,where   m > n

			options:
			    -m     <int>  | specifies lower bound of the
			    --mlo  <int>  | m component of the coprime pair
			                  |
			    -M     <int>  | specifies higher bound of the
			    --mhi  <int>  | m component of the coprime pair
			                  |
			    -n     <int>  | specifies lower bound of the
			    --nlo  <int>  | n component of the coprime pair
			                  |
			    -N     <int>  | specifies higher bound of the
			    --nhi  <int>  | n component of the coprime pair
			                  |
				-i     <int>  | specifies if, and how many id's
			    --ids  <int>  | generate, based on the coprimes
			                  |
			    -h     <int>  | asks to print this help text
			    --help <int>  |
			    --hepl <int>  |
			
			""";
		public static void Main(string[] args){
			Args.Args args1 = new(args);
			System.Console.WriteLine(args1.unrecognized_option_found);
			Action action = Action.UNRECOGNIZED;
			if(args1.unrecognized_option_found != null) {
				System.Console.WriteLine(Args.ExplainArgsErrors.ExplainUnrecognizedOptionFound((string)args1.unrecognized_option_found));
				return;
			}
			else if(args1.value_missing_for_option != null) {
				System.Console.WriteLine(Args.ExplainArgsErrors.ExplainValueMissingForOption((Args.ArgsParseState)args1.value_missing_for_option));
				return;
			}
			else if(args1.invalid_value_found != null) {
				System.Console.WriteLine(Args.ExplainArgsErrors.ExplainInvalidValueFound(((Args.ArgsParseState, string, Args.ArgsInvalidityType))args1.invalid_value_found));
				return;
			}
			else if(args1.m_lo != null && args1.m_hi != null) {
				action = Action.FIND_M_BETWEEN;
			}
			else if(args1.n_lo != null && args1.n_hi != null) {
				action = Action.FIND_N_BETWEEN;
			}
			else if(args1.hepl) {
				System.Console.Write(hepl_text);
				return;
			}
			//
			Coprimeses.Coprimes big_boi = new(Coprimeses.InitialVariants.InitialVariant.Even_Odd);
			if(action == Action.UNRECOGNIZED) {
				System.Console.Write(hepl_text);
				return;
			}
			else if(action == Action.FIND_M_BETWEEN) {
				Coprimeses.Coprimes? new_boi = Coprimeses.CoprimeFinder.CoprimeFinder.FindMBetween(Coprimeses.InitialVariants.InitialVariant.Even_Odd, (System.UInt128)args1.m_lo!, (System.UInt128)args1.m_hi!);
				if(new_boi == null) {
					System.Console.WriteLine("Coprime pair not found");
					return;
				}
				big_boi = new_boi;
				// if(big_boi != null) Console.WriteLine(big_boi.ToString());
				// else System.Console.WriteLine("Coprime pair not found");
			}
			else if(action == Action.FIND_N_BETWEEN) {
				Coprimeses.Coprimes? new_boi = Coprimeses.CoprimeFinder.CoprimeFinder.FindNBetween(Coprimeses.InitialVariants.InitialVariant.Even_Odd, (System.UInt128)args1.n_lo!, (System.UInt128)args1.n_hi!);
				if(new_boi == null) {
					System.Console.WriteLine("Coprime pair not found");
					return;
				}
				big_boi = new_boi;
				// if(big_boi != null) Console.WriteLine(big_boi.ToString());
				// else System.Console.WriteLine("Coprime pair not found");
			}
			//
			Console.WriteLine(big_boi.ToString());
			Ids.IdGenerator guwpy = new(big_boi);
			for(System.UInt32 i = 0; i < args1.ids; i++) System.Console.WriteLine((guwpy.Next() ?? new Ids.Id(0)).ToString());
		}
	}
	//
	//
	//
	//
	//
	//
	/**<summary>  The logic to parse commandline arguments.  </summary>**/
	//
	namespace Args {
		enum ArgsParseState{
			EXPECT_OPTION, UNRECOGNIZED_OPTION,
			HEPL,
			M_LO, M_HI, N_LO, N_HI,
			IDS,
		}
		enum ArgsInvalidityType{
			NOT_A_NUMBER, MISVALUED, MISRELATED_WITH_RANGE_BOUND, MISRELATED_BETWEEN_RANGE_BOUNDS
		}
		class Args {
			static readonly System.Collections.Generic.IReadOnlyDictionary<string, ArgsParseState> options = new System.Collections.Generic.Dictionary<string, ArgsParseState>{
				{"-m"    , ArgsParseState.M_LO},
				{"--mlo" , ArgsParseState.M_LO},
				{"-M"    , ArgsParseState.M_HI},
				{"--mhi" , ArgsParseState.M_HI},
				{"-n"    , ArgsParseState.N_LO},
				{"--nlo" , ArgsParseState.N_LO},
				{"-N"    , ArgsParseState.N_HI},
				{"--nhi" , ArgsParseState.N_HI},
				{"-h"    , ArgsParseState.HEPL},
				{"--help", ArgsParseState.HEPL},
				{"--hepl", ArgsParseState.HEPL},
				{"-i"    , ArgsParseState.IDS },
				{"--ids" , ArgsParseState.IDS },
			};
			public readonly System.UInt128? m_lo;
			public readonly System.UInt128? m_hi;
			public readonly System.UInt128? n_lo;
			public readonly System.UInt128? n_hi;
			public readonly System.UInt32 ids;
			public readonly bool hepl;
			public readonly string? unrecognized_option_found;
			public readonly ArgsParseState? value_missing_for_option;
			public readonly (ArgsParseState, string, ArgsInvalidityType)? invalid_value_found;
			public Args(string[] args) {
				this.m_lo                      = null;
				this.m_hi                      = null;
				this.n_lo                      = null;
				this.n_hi                      = null;
				this.ids                       = 0;
				this.hepl                      = false;
				this.unrecognized_option_found = null;
				this.invalid_value_found       = null;
				this.value_missing_for_option  = null;
				ArgsParseState stata = ArgsParseState.EXPECT_OPTION;
				foreach(string arg in args){
					switch(stata) {
						case ArgsParseState.EXPECT_OPTION:
							if(!options.TryGetValue(arg, out ArgsParseState next_state)) next_state = ArgsParseState.UNRECOGNIZED_OPTION;
							if(next_state == ArgsParseState.UNRECOGNIZED_OPTION) {
								this.unrecognized_option_found = arg;
								return;
							}
							else if(next_state == ArgsParseState.HEPL) {
								this.hepl = true;
								return;
							}
							else {
								stata = next_state;
								continue;
							}
						case ArgsParseState.M_LO:
							if(!System.UInt128.TryParse(arg, System.Globalization.NumberStyles.None, null, out UInt128 m_lo)){
								this.invalid_value_found = (ArgsParseState.M_LO, arg, ArgsInvalidityType.NOT_A_NUMBER);
								return;
							}
							if(this.m_hi != null) if(this.m_hi! <= m_lo) {
								this.invalid_value_found = (ArgsParseState.M_LO, arg, ArgsInvalidityType.MISRELATED_WITH_RANGE_BOUND);
								return;
							}
							this.m_lo = m_lo;
							stata = ArgsParseState.EXPECT_OPTION;
							continue;
						case ArgsParseState.M_HI:
							if(!System.UInt128.TryParse(arg, System.Globalization.NumberStyles.None, null, out UInt128 m_hi)){
								this.invalid_value_found = (ArgsParseState.M_HI, arg, ArgsInvalidityType.NOT_A_NUMBER);
								return;
							}
							if(this.m_lo != null) if(this.m_lo! >= m_hi) {
								this.invalid_value_found = (ArgsParseState.M_HI, arg, ArgsInvalidityType.MISRELATED_WITH_RANGE_BOUND);
								return;
							}
							if(this.n_lo != null) if(this.n_lo! >= m_hi) {
								this.invalid_value_found = (ArgsParseState.M_HI, arg, ArgsInvalidityType.MISRELATED_BETWEEN_RANGE_BOUNDS);
								return;
							}
							this.m_hi = m_hi;
							stata = ArgsParseState.EXPECT_OPTION;
							continue;
						case ArgsParseState.N_HI:
							if(!System.UInt128.TryParse(arg, System.Globalization.NumberStyles.None, null, out UInt128 n_hi)){
								this.invalid_value_found = (ArgsParseState.N_HI, arg, ArgsInvalidityType.NOT_A_NUMBER);
								return;
							}
							if(this.n_lo != null) if(this.n_lo! >= n_hi) {
								this.invalid_value_found = (ArgsParseState.N_HI, arg, ArgsInvalidityType.MISRELATED_WITH_RANGE_BOUND);
								return;
							}
							this.n_hi = n_hi;
							stata = ArgsParseState.EXPECT_OPTION;
							continue;
						case ArgsParseState.N_LO:
							if(!System.UInt128.TryParse(arg, System.Globalization.NumberStyles.None, null, out UInt128 n_lo)){
								this.invalid_value_found = (ArgsParseState.N_LO, arg, ArgsInvalidityType.NOT_A_NUMBER);
								return;
							}
							if(this.n_hi != null) if(this.n_hi! <= n_lo) {
								this.invalid_value_found = (ArgsParseState.N_LO, arg, ArgsInvalidityType.MISRELATED_WITH_RANGE_BOUND);
								return;
							}
							if(this.m_hi != null) if(this.m_hi! <= n_lo) {
								this.invalid_value_found = (ArgsParseState.N_LO, arg, ArgsInvalidityType.MISRELATED_BETWEEN_RANGE_BOUNDS);
								return;
							}
							this.n_lo = n_lo;
							stata = ArgsParseState.EXPECT_OPTION;
							continue;
						case ArgsParseState.IDS:
							if(!System.UInt32.TryParse(arg, System.Globalization.NumberStyles.None, null, out UInt32 ids)){
								this.invalid_value_found = (ArgsParseState.IDS, arg, ArgsInvalidityType.NOT_A_NUMBER);
								return;
							}
							if(ids == 0){
								this.invalid_value_found = (ArgsParseState.IDS, arg, ArgsInvalidityType.MISVALUED);
								return;
							}
							this.ids = ids;
							stata = ArgsParseState.EXPECT_OPTION;
							continue;
					}
				}
				if(stata != ArgsParseState.EXPECT_OPTION) {
					this.value_missing_for_option = stata;
					return;
				}
				return;
			}
		}
		static class ExplainArgsErrors{
			static readonly System.Collections.Generic.IReadOnlyDictionary<ArgsParseState, System.String> args_parse_state = new System.Collections.Generic.Dictionary<ArgsParseState, System.String>{
				{ArgsParseState.M_HI, "m high bound"},
				{ArgsParseState.M_LO, "m low bound"},
				{ArgsParseState.N_HI, "n high bound"},
				{ArgsParseState.N_LO, "n low bound"},
				{ArgsParseState.IDS , "ids to generate"},
			};
			public static String ExplainUnrecognizedOptionFound(string error){
				System.Text.StringBuilder repr = new("unrecognized \"");
				repr.Append(error);
				repr.Append("\" option provided");
				return repr.ToString();
			}
			public static String ExplainValueMissingForOption(ArgsParseState error){
				System.Text.StringBuilder repr = new("expected a value for option \"");
				if(args_parse_state.TryGetValue(error, out System.String? repri)) repr.Append(repri);
				else repr.Append("other");
				repr.Append("\" but found the end of options");
				return repr.ToString();
			}
			public static String ExplainInvalidValueFound((ArgsParseState, string, ArgsInvalidityType) error){
				System.Text.StringBuilder repr = new("provided value (");
				repr.Append(error.Item2);
				repr.Append(") for \"");
				if(args_parse_state.TryGetValue(error.Item1, out System.String? stata)) repr.Append(stata);
				else repr.Append("(unexpected)");
				switch(error.Item3){
					case ArgsInvalidityType.NOT_A_NUMBER:
						repr.Append("\" is not a valid positive integer number");
						break;
					case ArgsInvalidityType.MISVALUED:
						repr.Append("\" is numerically correct, but doesn't make sense as a value for the option");
						break;
					case ArgsInvalidityType.MISRELATED_WITH_RANGE_BOUND:
						repr.Append("\" is numerically correct, but doesn't make sense as a second bound of a range");
						break;
					case ArgsInvalidityType.MISRELATED_BETWEEN_RANGE_BOUNDS:
						repr.Append("\" is numerically correct, but doesn't make sense as a bound relating to another range");
						break;
				}
				return repr.ToString();
			}
		}
	}
}
