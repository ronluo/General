﻿
		private $COLUMNTYPE$ _$COLUMNNAME$;
		/// <summary>
		/// $COLUMNDESCRIPTION$.
		/// </summary>
#if SERIALIZATION_DATACONTRACT
        [DataMember]
#endif
		[System.ComponentModel.Description("$COLUMNDESCRIPTION$")]     
		public $COLUMNTYPE$ $COLUMNNAME$
		{
			get 
			{
				return this._$COLUMNNAME$;
			}
			set 
			{
				this._$COLUMNNAME$ = value;
			}
		}
