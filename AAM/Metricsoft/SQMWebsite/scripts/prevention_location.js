function OnClientClicked(sender, eventArgs) {

	var radGrid = $find(sender.get_id().replace("rbSelectAll", "rgPlantContacts"));
	var masterTable = radGrid.get_masterTableView();
	var i;

	if (sender.get_text() === "Select All") {
		//sender.set_text("Clear All");
		for (i = 0; i < masterTable.get_dataItems().length; i++) {
			masterTable.selectItem(i);
		}
	} else {
		//sender.set_text("Select All");
		for (i = 0; i < masterTable.get_dataItems().length; i++) {
			masterTable.get_dataItems()[i].set_selected(false);
		}
	}

}


function RowSelectedChanged(sender, eventArgs) {
	
	var radGrid = sender;
	var masterTable = radGrid.get_masterTableView();
	var count = 0;

	for (var i = 0; i < masterTable.get_dataItems().length; i++) {
		if (masterTable.get_dataItems()[i].get_selected() === true)
			count++;
	}

	var radButton = $find(sender.get_id().replace("rgPlantContacts", "rbSelectAll"));

	if (count === 0)
		radButton.set_text("Select All");

	if (count === masterTable.get_dataItems().length)
		radButton.set_text("Clear All");
	
}