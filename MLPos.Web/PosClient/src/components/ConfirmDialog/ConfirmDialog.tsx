import "./ConfirmDialog.css";
function ConfirmDialog({ message, onConfirm, onCancel }: ConfirmDialogProps) {
  return (
    <div className="modal">
      <div className="modalContent confirmDialog">
        <div className="confirmDialogMessage">{message}</div>
        <div className="confirmDialogButtons">
          <button
            className="button buttonPrimary confirmButton"
            onClick={onConfirm}
          >
            Confirm
          </button>
          <button
            className="button buttonSecondary cancelButton"
            onClick={onCancel}
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

export default ConfirmDialog;
