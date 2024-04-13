using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pakaian
{
    private string id_pakaian;
    protected string gambar;
    protected string nama;
    protected int harga;
    protected string tipe;
    protected string xpos;
    protected string ypos;
    protected float xscale;
    protected float yscale;
    public string Id_pakaian { get => id_pakaian; set => id_pakaian = value; }
    public string Gambar { get => gambar; set => gambar = value; }
    public string Nama { get => nama; set => nama = value; }
    public int Harga { get => harga; set => harga = value; }
    public string Tipe { get => tipe; set => tipe = value; }
    public string Xpos { get => xpos; set => xpos = value; }
    public string Ypos { get => ypos; set => ypos = value; }
    public float Xscale { get => xscale; set => xscale = value; }
    public float Yscale { get => yscale; set => yscale = value; }

    public Pakaian(string id_pakaian, string gambar, string harga, string nama, string tipe, string xpos, string ypos, float xscale, float yscale){
        this.id_pakaian = id_pakaian;
        this.gambar = gambar;
        this.nama = nama;
        this.harga = int.Parse(harga);
        this.tipe = tipe;
        this.xpos = xpos;
        this.ypos = ypos;
        this.xscale = xscale;
        this.yscale = yscale;
    }
}
