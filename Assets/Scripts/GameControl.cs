using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameControl : MonoBehaviour
{
    public GameManager game_manager;

    [Header("Tilemaps")]
    public Tilemap grid;
    public Tilemap figures_grid;
    public Tilemap highlight_grid;
    public Tilemap player_color_grid;

    [Header("Tiles")]
    public Tile laser;
    public Tile prizm;
    public Tile mirror;
    public Tile pillar;
    public Tile moving;
    public Tile select;
    public Tile[] player_color;

    [Header("Beam")]
    public LineRenderer beam_renderer;
    public GameObject laser_emition_prefab;

    [HideInInspector]
    public bool rotating = false;

    List<Figure> figures;
    bool figure_selected;
    bool figure_moved = false;
    Figure current_figure;
    Vector3Int cur_grid_pos = Vector3Int.zero;
    Vector3Int prev_grid_pos = Vector3Int.zero;

    GameObject rot_panel;
    GameObject[] emition = new GameObject[2]; //emition and hit effects
    
    private void Start()
    {
        game_manager = GetComponentInParent<GameManager>();
        rot_panel = game_manager.GetRotationPanel();
        rot_panel.SetActive(false);
        figures = new List<Figure>();
        InitFigures();
    }
    private void Update()
    {
        if (!game_manager.isPlaying)
            return;

        Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.E))
            Rotate_Figure(true); //right
        if (Input.GetKeyDown(KeyCode.Q))
            Rotate_Figure(false);  //left
        if (Input.GetKeyDown(KeyCode.Escape))
            Unselect();

        if (Input.GetMouseButtonDown(0))
        {
            prev_grid_pos = cur_grid_pos;
            cur_grid_pos = figures_grid.WorldToCell(mpos);

            if (!rotating)
                ProcSelectedCell();
            else
                Debug.Log("rotation is active");
        }
    }

    private void ProcSelectedCell()
    {
        
        if (figures_grid.HasTile(cur_grid_pos))
        {
            Figure selected_figure = GetFigureFromPos(cur_grid_pos);
            Debug.Log(selected_figure.player +" cmp " + game_manager.active_player);
            if (selected_figure.player == game_manager.active_player)
            {
                current_figure = selected_figure;

                if (figure_selected)
                    highlight_grid.ClearAllTiles();

                figure_selected = true;
                Debug.Log(current_figure.type + " selected");
                highlight_grid.SetTile(current_figure.position, select);
                rot_panel.SetActive(true);
                if (!figure_moved)
                    HighLightPossableMove(current_figure.position);
            }           
        }
        else
        {
            if (figure_selected)
                if (highlight_grid.HasTile(cur_grid_pos))
                {
                    current_figure.position = cur_grid_pos;
                    MoveTileOnGrid(current_figure, prev_grid_pos);
                    highlight_grid.ClearAllTiles();
                    highlight_grid.SetTile(current_figure.position, select);
                    figure_moved = true;
                }
        }
    }
   
    private void InitFigures()
    {       
        for(int player = 1; player <= 2; player++)
        {
            figures.Add(new Figure(FigureType.Lazer, 0, player));
            figures.Add(new Figure(FigureType.Pillar, 0, player));

            int prizm_count = DefaultFigurePositions.Prizm.Length;
            for (int x = 0; x < prizm_count; x++)
                figures.Add(new Figure(FigureType.Prizm, x, player));

                

            int mirror_count = DefaultFigurePositions.Mirror.Length;
            for (int x = 0; x < mirror_count; x++)
                figures.Add(new Figure(FigureType.Mirror, x, player));

        }
        

        //placing figures on the field
        foreach(var f in figures)
        {           
            //figures_grid.SetTile(f.position, GetTileFromType(f.type));
            MoveTileOnGrid(f, Vector3Int.zero);
        }           
    }

    private void HighLightPossableMove(Vector3Int start)
    {
        Cell cell = new Cell();
        for (int x = 0; x < 6; x++)  
        {
            cell.SetPosition(start);
            HighlightLine(cell, x);
        }
            
    }

    private void HighlightLine(Cell cell, int direction) 
    {
        Tile gridtile;
        Tile figuretile;

        while (true)
        {
            cell.Move(direction);
            gridtile = (Tile)grid.GetTile(cell.position);
            figuretile = (Tile)figures_grid.GetTile(cell.position);
            if (gridtile == null || figuretile != null)
                break;
            highlight_grid.SetTile(cell.position, moving);
        }
    }

    private void MoveTileOnGrid(Figure f, Vector3Int old_pos)
    {
        Debug.Log("set " + game_manager.GetPlayerName(f.player) + "'s " + f.type + " to " + f.position);
        figures_grid.SetTile(f.position, GetTileFromType(f.type));
        player_color_grid.SetTile(f.position, player_color[(f.player) - 1]);
        SyncTileRotation(f);
        figures_grid.SetTile(old_pos, null);
        player_color_grid.SetTile(old_pos, null);
    }

    public void RotateCELL_CKW()
    {
        Rotate_Figure(true);
    }

    public void RotateCELL_CounterCKW()
    {
        Rotate_Figure(false);
    }
    private void Rotate_Figure(bool right)
    {
        if (!figure_selected)
            return;

        current_figure.direction += right ? -1 : 1;

        if (current_figure.direction == -1)  // [0:5] possable rotations
            current_figure.direction = 5;
        if (current_figure.direction == 6)
            current_figure.direction = 0;

        Debug.Log("direction of " + current_figure.type + " = " + current_figure.direction);
        SyncTileRotation(current_figure);
    }

    private void SyncTileRotation(Figure f)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, f.direction * 60f), Vector3.one);
        figures_grid.SetTransformMatrix(f.position, matrix);
    }
    private void Unselect()
    {
        Debug.Log("cancel");
        figure_selected = false;
        current_figure = null;
        highlight_grid.ClearAllTiles();
        rot_panel.SetActive(false);
    }

    public void CalculateBeam()
    {
        Unselect();
        game_manager.HideRotBtns();

        rot_panel.SetActive(false);

        List<Figure> lasers = figures.FindAll(x => x.type == FigureType.Lazer);//lasers
        Figure curent_laser = lasers.Find(x => x.player == game_manager.active_player);
        List<Vector3> beamcoords = new List<Vector3>();
        int beamdir = curent_laser.direction;
        beamcoords.Add(grid.CellToWorld(curent_laser.position));
        bool beam_end = false;
        Cell beam = new Cell(curent_laser.position);
        do
        {
            beam.Move(beamdir);
            if (beam_end || !grid.HasTile(beam.position)) //beam has been not reflected or out of field
            {
                beamcoords.Add(grid.CellToWorld(beam.position));
                break;
            }
            
            if(figures_grid.HasTile(beam.position)) // laser's hit proccessing (reflecting or damaging)
            {
                Figure proc_figure = GetFigureFromPos(beam.position);
                beamcoords.Add(grid.CellToWorld(beam.position));
                beamdir = proc_figure.Reflect(beamdir);  //all magic here
                if(beamdir == 10) //10 is no reflect
                {
                    beam_end = true;
                    if (proc_figure.type == FigureType.Pillar)
                        game_manager.PillarHit(proc_figure.player);
                }
                    
            }

        } while (grid.HasTile(beam.position));

        emition[0] = Instantiate(laser_emition_prefab, beamcoords[0], Quaternion.identity);
        emition[1] = Instantiate(laser_emition_prefab, beamcoords[beamcoords.Count-1], Quaternion.identity);
        beam_renderer.positionCount = beamcoords.Count;
        beam_renderer.SetPositions(beamcoords.ToArray());
        figure_moved = false;
        StartCoroutine("EmitLaser");
    }

    IEnumerator EmitLaser()
    {
        for (int i=0;i<30;i++)
        {
            if (i == 29)
            {
                Destroy(emition[0]);
                Destroy(emition[1]);
                game_manager.EndTurn();
                beam_renderer.positionCount = 0;               
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    private Tile GetTileFromType(FigureType type)
    {
        switch(type)
        {
            case FigureType.Pillar: return pillar;
            case FigureType.Lazer: return laser;
            case FigureType.Mirror: return mirror;
            case FigureType.Prizm: return prizm;
            default: return null;
        };
    }

    private Figure GetFigureFromPos(Vector3Int position)
    {
        return figures.Find(x => x.position == position);
    }
}
