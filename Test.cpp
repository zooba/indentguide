#define PRAGMA

using System;

namespace Namespace

{
    
    class Class
    
    {

        void Func1(int param1,

            int param2)
        
        {

			// Do something

        }

#ifdef PRAGMA

        void Func2(int param1,

                   int param2,

                   int param3)

        {

			// Do something

		}

#else

        void Func2(int param1,

            int param2,

            int param3)

        {

			// Do something

		}

#endif

        #pragma region Region

        void Func3()
        {

            if (true) {

				// Do something

            } else {

				// Do something

            }

        }

        #pragma endregion

    };

}
