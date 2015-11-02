using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WHY_Shadow
{
    public partial class Parameter : Form
    {
        public Parameter()
        {
            InitializeComponent();
        }

        public int val
        {
            set
            {
                this.trackBar1.Value = value;
            }
            get
            {
                return this.trackBar1.Value;
            }
        }
    }
}
