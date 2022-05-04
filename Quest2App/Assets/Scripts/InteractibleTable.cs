using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class InteractibleTable : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject menu;
    private LineRenderer lineRenderer;
    void Start()
    {
        menu = Instantiate(menu, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
        if(PhotonNetwork.IsMasterClient)
        {
            menu.transform.position = transform.position + (menu.transform.up * -(transform.localScale.z * 4.0f));
        } else
        {
            menu.transform.position = transform.position + (menu.transform.up * (transform.localScale.z * 4.0f));
            menu.transform.Rotate(menu.transform.position, 180);
        }
        
        lineRenderer = GetComponent<LineRenderer>();
        SetColourScheme();
    }

    void Update()
    {

        UpdateEdgeLines();

    }

    private void UpdateEdgeLines()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        lineRenderer.SetPosition(0, transform.TransformPoint(vertices[0]));
        lineRenderer.SetPosition(1, transform.TransformPoint(vertices[10]));
        lineRenderer.SetPosition(3, transform.TransformPoint(vertices[110]));
        lineRenderer.SetPosition(2, transform.TransformPoint(vertices[120]));

    }

    private void SetColourScheme()
    {
        if (gameObject.GetPhotonView().AmOwner)
        {
            lineRenderer.material.color = new Color(0.34f, 0.76f, 0.31f);
            GetComponentInChildren<Image>().color = new Color(0.34f, 0.76f, 0.31f, 0f); // Currently invisible surface
        } else
        {
            lineRenderer.material.color = new Color(0.31f, 0.32f, 0.73f);
            GetComponentInChildren<Image>().color = new Color(0.31f, 0.32f, 0.73f, 0f); // Currently invisible surface
            menu.SetActive(false);
        }
    }

    public void SpawnBox()
    {
        PhotonNetwork.Instantiate("NetworkCube", GetRandomSpawnPoint(), transform.rotation, 0);
    }

    public void SpawnSphere()
    {
        PhotonNetwork.Instantiate("NetworkSphere", GetRandomSpawnPoint(), transform.rotation, 0);
    }

    public void SpawnPaper()
    {
        PhotonNetwork.Instantiate("NetworkPaper", GetRandomSpawnPoint(), Quaternion.Euler(new Vector3(90, 90, 0)), 0);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        return transform.position + new Vector3(Random.Range(-0.1f, 0.2f), Random.Range(0.5f, 1f), Random.Range(-0.1f, 0.2f));
    }
    
}
