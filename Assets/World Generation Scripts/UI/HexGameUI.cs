using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;

	HexUnit selectedUnit;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (selectedUnit) {
				if (Input.GetMouseButtonDown(1)) {
					DoMove();
				}
				else {
					DoPathfinding();
				}
			}
		}
	}

	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
		}
	}

	public void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
			}
			else {
				grid.ClearPath();
			}
		}
	}
	
	public void DoPathfinding(HexUnit unit, HexCell cell) {
		if (cell && unit.IsValidDestination(cell)) {
			grid.FindPath(unit.Location, cell, unit);
		}
		else {
			grid.ClearPath();
		}
	}

	public void DoMove () {
		if (grid.HasPath) {
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}
	
	public void DoMove(HexUnit unit) {
		if (grid.HasPath) {
			unit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}

	public bool UpdateCurrentCell() {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
	
	public bool UpdateCurrentCell(HexCell cell) {
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}