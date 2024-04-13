using System;
using System.Drawing;
using System.Windows.Forms;

public class DualTrackBar : Control
{
    private int _minValue = 0;
    private int _maxValue = 100;
    private int _lowerValue = 0;
    private int _upperValue = 100;
    private int _movingThumb = 0; // 0 - ни один, 1 - нижний, 2 - верхний

    private const int ThumbSize = 16; // Размеры указателей

    public int MinimumValue
    {
        get { return _minValue; }
        set
        {
            _minValue = value;
            Invalidate();
        }
    }

    public int MaximumValue
    {
        get { return _maxValue; }
        set
        {
            _maxValue = value;
            Invalidate();
        }
    }

    public int LowerValue
    {
        get { return _lowerValue; }
        set
        {
            _lowerValue = Math.Min(value, _upperValue);
            Invalidate();
        }
    }

    public int UpperValue
    {
        get { return _upperValue; }
        set
        {
            _upperValue = Math.Max(value, _lowerValue);
            Invalidate();
        }
    }

    public DualTrackBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint |
                 ControlStyles.Selectable, true);

        MouseDown += DualTrackBar_MouseDown;
        MouseUp += DualTrackBar_MouseUp;
        MouseMove += DualTrackBar_MouseMove;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        Rectangle clientRect = ClientRectangle;

        int range = _maxValue - _minValue;
        int lowerPos = (int)((_lowerValue - _minValue) / (double)range * (clientRect.Width - ThumbSize * 2));
        int upperPos = (int)(((double)(_upperValue - _minValue) / range) * (clientRect.Width - ThumbSize * 2));

        // Рисуем внешнюю светлую линию
        using (Pen pen = new Pen(SystemColors.ControlLight, 2))
        {
            g.DrawLine(pen, ThumbSize / 2, clientRect.Height / 2, clientRect.Width - ThumbSize / 2, clientRect.Height / 2);
        }

        // Рисуем внутреннюю темную линию между указателями
        using (Pen pen = new Pen(SystemColors.ControlDark, 2))
        {
            g.DrawLine(pen, lowerPos + ThumbSize, clientRect.Height / 2, upperPos + ThumbSize, clientRect.Height / 2);
        }

        // Рисуем четкие разметки
        for (int i = _minValue; i <= _maxValue; i += (_maxValue - _minValue) / 10)
        {
            int xPos = (int)((i - _minValue) / (double)range * (clientRect.Width - ThumbSize * 2));
            g.DrawLine(SystemPens.ControlDark, xPos + ThumbSize, clientRect.Height / 2 - 4, xPos + ThumbSize, clientRect.Height / 2 + 4);
        }

        // Рисуем полосу индикации между двумя указателями
        using (SolidBrush brush = new SolidBrush(Color.LightGray))
        {
            g.FillRectangle(brush, lowerPos + ThumbSize, clientRect.Height / 2 - 2, upperPos - lowerPos, 4);
        }

        // Рисуем два указателя
        using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 122, 204))) // Цвет указателей (синий)
        {
            g.FillRectangle(brush, lowerPos, clientRect.Height / 2 - ThumbSize / 2, ThumbSize, ThumbSize);
            g.FillRectangle(brush, upperPos + ThumbSize, clientRect.Height / 2 - ThumbSize / 2, ThumbSize, ThumbSize);
        }
    }

    private void DualTrackBar_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            int range = _maxValue - _minValue;
            int lowerPos = (int)((_lowerValue - _minValue) / (double)range * (ClientRectangle.Width - ThumbSize * 2));
            int upperPos = (int)((_upperValue - _minValue) / (double)range * (ClientRectangle.Width - ThumbSize * 2));

            Rectangle lowerThumbRect = new Rectangle(lowerPos, ClientRectangle.Height / 2 - ThumbSize / 2, ThumbSize, ThumbSize);
            Rectangle upperThumbRect = new Rectangle(upperPos + ThumbSize, ClientRectangle.Height / 2 - ThumbSize / 2, ThumbSize, ThumbSize);

            if (lowerThumbRect.Contains(e.Location))
            {
                _movingThumb = 1;
            }
            else if (upperThumbRect.Contains(e.Location))
            {
                _movingThumb = 2;
            }
        }
    }

    private void DualTrackBar_MouseMove(object sender, MouseEventArgs e)
    {
        if (_movingThumb == 1 || _movingThumb == 2)
        {
            int range = _maxValue - _minValue;
            int clickValue = (int)((e.X - ThumbSize) / (double)(ClientRectangle.Width - ThumbSize * 2) * range + _minValue);

            if (_movingThumb == 1)
            {
                _lowerValue = Math.Max(_minValue, Math.Min(clickValue, _upperValue));
            }
            else if (_movingThumb == 2)
            {
                _upperValue = Math.Min(_maxValue, Math.Max(clickValue, _lowerValue));
            }

            Invalidate();
        }
    }

    private void DualTrackBar_MouseUp(object sender, MouseEventArgs e)
    {
        _movingThumb = 0;
    }
}