//
//
//
//
//
//
/**<summary>  Things related to id's.  </summary>**/
//
namespace Ids {
	//
	//
	//
	//
	//
	//
	/**<summary>  The id type.  </summary>**/
	//
	class Id: System.IComparable<Id> {
		readonly System.UInt128 value;
		public Id(System.UInt128 value){
			this.value = value;
		}

		public int CompareTo(Id? other)
		{
			if(other == null)                 return  1;
			else if(this.value > other.value) return  1;
			else if(this.value < other.value) return -1;
			else                              return  0;
		}
		public override string ToString()
		{
			System.Text.StringBuilder repr = new("Id{");
			repr.Append(this.value.ToString());
			repr.Append('}');
			return repr.ToString();
		}
	}
	//
	//
	//
	//
	//
	//
	/**<summary>  The id generator, using coprime pairs.  </summary>**/
	//
	class IdGenerator {
		System.UInt128 next_value;
		readonly System.UInt128 inkrement;
		readonly System.UInt128 mask;
		readonly System.UInt128 modulus;
		public static System.UInt128 NextRandomUint128(System.Random rng){
			return ((System.UInt128)rng.Next(1<<30))
				 | ((System.UInt128)rng.Next(1<<30) << 30)
				 | ((System.UInt128)rng.Next(1<<30) << 60)
				 | ((System.UInt128)rng.Next(1<<30) << 90)
				 | ((System.UInt128)rng.Next(1<<8) << 120);
		}
		public IdGenerator(Coprimeses.Coprimes seed){
			System.Random rng = System.Random.Shared;
			this.modulus = seed.M;
			this.inkrement = seed.N;
			this.mask = NextRandomUint128(rng);
			this.next_value = this.inkrement;
		}
		public Id? Next(){
			//( out of id's problem )
			if(this.next_value == 0) return null;
			//( normal flow )
			Id next = new(this.next_value ^ this.mask);
			this.next_value = (this.next_value + this.inkrement) % this.modulus;
			return next;
		}
	}
}