using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonTranProfile
{
    public class Resume {
	
	    public const string FullName = "SON N. TRAN";
	    protected string Email = "esente@gmail.com";
	    protected string UsPhone = "+1 405 872 6766";
        protected string VnPhone = "+84 169 214 1616";

        public void Objective() {
            const string objective = "A position in the computer and technology field" +
                                     "to apply personal knowledge and experience" + 
                                     "in real working environment.";
            Console.WriteLine(objective);
        }

        public void Skills() {
            
        }
    }
}
