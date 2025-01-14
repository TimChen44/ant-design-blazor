﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AntDesign
{
    public partial class Step : AntDomComponentBase
    {
        private string _status = "wait";
        private bool _isCustomStatus;
        private int _groupCurrent;

        private readonly Dictionary<string, object> _containerAttributes = new Dictionary<string, object>();

        internal bool Clickable { get; set; }

        internal bool Last { get; set; }

        internal bool ShowProcessDot { get; set; }

        internal string GroupStatus { get; set; } = string.Empty;

        internal int GroupCurrentIndex
        {
            get => _groupCurrent;
            set
            {
                _groupCurrent = value;
                if (!_isCustomStatus)
                {
                    this._status = value > this.Index ? "finish" : value == this.Index ? GroupStatus ?? string.Empty : "wait";
                }
                InvokeStateHasChanged();
            }
        }

        internal int Index { get; set; }
        internal double? Percent { get; set; }
        internal string Size { get; set; } = "default";
        internal RenderFragment ProgressDot { get; set; }
        internal string Direction { get; set; } = "horizontal";

        [CascadingParameter]
        public Steps Parent { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    _isCustomStatus = true;
                    InvokeStateHasChanged();
                }
            }
        }

        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public RenderFragment TitleTemplate { get; set; }

        [Parameter] public string Subtitle { get; set; } = string.Empty;

        [Parameter] public RenderFragment SubtitleTemplate { get; set; }

        [Parameter] public string Description { get; set; } = string.Empty;

        [Parameter] public RenderFragment DescriptionTemplate { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter] public bool Disabled { get; set; }

        protected override void OnInitialized()
        {
            Parent?.AddStep(this);

            SetClassMap();
            if (Clickable && !Disabled)
            {
                _containerAttributes["role"] = "button";
            }
        }

        protected override void Dispose(bool disposing)
        {
            Parent._children.Remove(this);
            Parent.ResetChildrenSteps();
            base.Dispose(disposing);
        }

        internal int? GetTabIndex()
        {
            if (!Disabled && Clickable)
                return 0;
            else
                return null;
        }

        protected void SetClassMap()
        {
            string prefixName = "ant-steps-item";
            ClassMapper.Clear()
                .Add(prefixName)
                .GetIf(() => $"{prefixName}-{Status}", () => !string.IsNullOrEmpty(Status))
                .If($"{prefixName}-active", () => Parent.Current == Index)
                .If($"{prefixName}-disabled", () => Disabled)
                .If($"{prefixName}-custom", () => !string.IsNullOrEmpty(Icon))
                .If($"ant-steps-next-error", () => GroupStatus == "error" && Parent.Current == Index + 1)
                ;
        }

        private void HandleClick(MouseEventArgs args)
        {
            if (Clickable && !Disabled)
            {
                Parent.NavigateTo(Index);
                if (OnClick.HasDelegate)
                {
                    OnClick.InvokeAsync(args);
                }
            }
        }
    }
}
