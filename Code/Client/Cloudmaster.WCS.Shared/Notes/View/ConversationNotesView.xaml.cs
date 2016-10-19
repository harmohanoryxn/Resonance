using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCS.Shared.Schedule;

namespace WCS.Shared.Notes
{
	public partial class ConversationNotesView : UserControl
	{
		public ConversationNotesView()
		{
			InitializeComponent();
		}

		private void txtNote_KeyDown(object sender, KeyEventArgs e)
		{
			// if user didn't press Enter, do nothing
			if (!e.Key.Equals(Key.Enter)) return;

			var dc = DataContext as ConversationNotesViewModel;
			if (dc == null) return;

			dc.AddNewNoteCommand.Execute(null);
			dc.DismissCommand.Execute(null);

			var topLevelContainer = this.TryFindLogicalParent<ScreenBootstrap>();
			if (topLevelContainer != null)
			{
				var cdc = topLevelContainer.DataContext as WcsViewModel;
				if (cdc == null) return;
				cdc.ClearAllSelections();
			}
		}

		private void txtNote_LostFocus(object sender, RoutedEventArgs e)
		{
			var dc = DataContext as ConversationNotesViewModel;
			if (dc == null) return;

			dc.DismissCommand.Execute(null);

		}

	 

		/// <summary>
		/// This is the first part of a hack that puts focus into the notes textbox when its selected. This part of the hack calls the second part of the hack
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void cnv_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				var dc = DataContext as ConversationNotesViewModel;
				if (dc == null) return;
				dc.ForceFocusToTextbox();
			}
		}
	}
}
