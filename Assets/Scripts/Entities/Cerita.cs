using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cerita
{
    private string id_dokumen;
    private int id_cerita;
    private int variasi_adegan;
    protected string thumbnail;
    protected string title;
    protected string type;
    public string Id_dokumen { get => id_dokumen; set => id_dokumen = value; }
    public int Id_cerita { get => id_cerita; set => id_cerita = value; }
    public int Variasi_adegan { get => variasi_adegan; set => variasi_adegan = value; }

    public Cerita(string id_dokumen, int id_cerita, string thumbnail, string title, string type, int variasi_adegan)
    {
        this.id_dokumen = id_dokumen;
        this.id_cerita = id_cerita;
        this.thumbnail = thumbnail;
        this.title = title;
        this.type = type;
        this.variasi_adegan = variasi_adegan;
    }

    public Cerita(string id_dokumen, int id_cerita, string thumbnail, string title, string type)
    {
        this.id_dokumen = id_dokumen;
        this.id_cerita = id_cerita;
        this.thumbnail = thumbnail;
        this.title = title;
        this.type = type;
    }
}
