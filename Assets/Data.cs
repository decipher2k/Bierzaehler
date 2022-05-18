using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Data : MonoBehaviour
{
    public GameObject line;
    bool started = false;

    private Dictionary<String, int> data = new Dictionary<string, int>();
    public string getDateBeforeNow(int offset=0)
    {
        return DateTime.Today.AddDays(offset).ToString();
    }

    private int getNumLines()
    {
        String[] lines = File.ReadAllLines(Application.persistentDataPath+"/data");
        return (lines.Length);
    }

    private void saveData()
    { 
        // you can play around with the path if you like, but build-vs-run locations need to be taken into account
		if(!Directory.Exists("/sdcard/.bz"))
        {
        	//Directory.CreateDirectory("/sdcard/.bz");
        }

        if (File.Exists(Application.persistentDataPath + "/data"))
            File.Delete(Application.persistentDataPath + "/data");
      
        String txt = "";

        for (int i = 0; i > -7; i--)
        {
            int value = QueryInt(getDateBeforeNow(i));
            if (value == -1)
                value = 0;
            txt += getDateBeforeNow(i) + ";" + value.ToString() + "\r\n";        
        }
        File.WriteAllText(Application.persistentDataPath + "/data", txt);
    }

    private void loadData()
    {
        try
        {
            String[] lines = File.ReadAllLines(Application.persistentDataPath + "/data");
            foreach (String line in lines)
            {
                if (line.Split(';').Length > 0)
                    data.Add(line.Split(';')[0], int.Parse(line.Split(';')[1]));
            }
        }
        catch (Exception ex) { }
    }

    private int QueryInt(String date)
    {
        if (data.ContainsKey(date))
            return data[date];
        else
            return -1;
    }

    private void ExecuteNonQuery(String date, int Value)
    {
        if (data.ContainsKey(date))
            data[date] = Value;
        else
            data.Add(date, Value);

        saveData();


    }

    public void updateData()
    {
        GameObject.Find("DataDiagram").GetComponent<DD_DataDiagram>().DestroyLine(this.line);
        line = GameObject.Find("DataDiagram").GetComponent<DD_DataDiagram>().AddLine("1", Color.red);
        GameObject.Find("DataDiagram").GetComponent<DD_DataDiagram>().InputPoint(line, new Vector2(1, 0));

        for (int i = 0; i>=-6; i--)
        {
        
                int amount = QueryInt( getDateBeforeNow(i));
            if (amount == -1)
                amount = 0;
                GameObject.Find("DataDiagram").GetComponent<DD_DataDiagram>().InputPoint(line, new Vector2(1, amount));
          
        }

        if (QueryInt( getDateBeforeNow(-1)) == -1)
            GameObject.Find("txtGestern").GetComponent<TMPro.TextMeshProUGUI>().text = "<keine Daten>";
        else
            GameObject.Find("txtGestern").GetComponent<TMPro.TextMeshProUGUI>().text = QueryInt(getDateBeforeNow(-1)).ToString();

        if (QueryInt( getDateBeforeNow(0)) == -1)
            GameObject.Find("txtHeute").GetComponent<TMPro.TextMeshProUGUI>().text = "0";
        else
            GameObject.Find("txtHeute").GetComponent<TMPro.TextMeshProUGUI>().text =QueryInt(getDateBeforeNow(0)).ToString();

        if (QueryInt( getDateBeforeNow(-1)) >= QueryInt( getDateBeforeNow(0)))
            GameObject.Find("txtHeute").GetComponent<TMPro.TextMeshProUGUI>().color = Color.green;
        else
            GameObject.Find("txtHeute").GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;

        
    }

    // Start is called before the first frame update
    void Start()
    {
        loadData();
        line = GameObject.Find("DataDiagram").GetComponent<DD_DataDiagram>().AddLine("1", Color.red);
        GameObject.Find("DataDiagram").GetComponent<DD_DataDiagram>().InputPoint(line, new Vector2(1, 0));
        updateData();

    }

    public void addBeer()
    {
        int sAmount = QueryInt(getDateBeforeNow(0));
        if(sAmount<0)
        {
            ExecuteNonQuery(getDateBeforeNow(0), 1);
        }
        else
        {
            int amount = QueryInt( getDateBeforeNow(0));
            amount++;
            ExecuteNonQuery(getDateBeforeNow(0), amount);
        }
        

        updateData();
    }

    public void deleteBeer()
    {
        int amount = QueryInt(getDateBeforeNow(0));
        if (amount > 0)
            {
                amount--;
               ExecuteNonQuery(getDateBeforeNow(0), amount);
            }        

        updateData();
    }



    // Update is called once per frame
    void Update()
    {

    }
}
