using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	private Quaternion correctPlayerHeadRot = Quaternion.identity; // We lerp towards this

	private HunterController hc;

	void Awake() {
		hc = GetComponent<HunterController>();
		if(!photonView.isMine) {
			GetComponent<CharacterMotor>().enabled = false;
		}
	}

    void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
			hc.headPivot.transform.rotation = Quaternion.Lerp(hc.headPivot.transform.rotation, this.correctPlayerHeadRot, Time.deltaTime * 5);
		}
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
			stream.SendNext(hc.headPivot.transform.rotation);
        } else {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			this.correctPlayerHeadRot = (Quaternion)stream.ReceiveNext();
        }
    }

	public void IgnoreLerp() {
		this.correctPlayerPos = transform.position;
		this.correctPlayerRot = transform.rotation;
		this.correctPlayerHeadRot = hc.headPivot.transform.rotation;
	}
}
