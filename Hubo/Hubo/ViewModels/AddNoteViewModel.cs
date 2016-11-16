using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace Hubo
{
    class AddNoteViewModel
    {
        public INavigation Navigation { get; set; }
        public string SaveText { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public ICommand SaveCommand { get; set; }
        public string Note { get; set; }

        DatabaseService DbService = new DatabaseService();
        public AddNoteViewModel()
        {
            SaveText = Resource.Save;
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
            SaveCommand = new Command(SaveNote);
        }

        private void SaveNote()
        {
            DbService.SaveNote(Note, Date, Time);
            Navigation.PopAsync();                        
        }
    }
}
