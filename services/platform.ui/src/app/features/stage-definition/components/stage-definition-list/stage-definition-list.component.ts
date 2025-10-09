import { Component, inject, OnInit } from '@angular/core';
import { NzTreeNodeOptions, NzTreeModule, NzFormatEmitEvent } from 'ng-zorro-antd/tree';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';
import { StageDefinitionService } from '../../services/stage-definition.service';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { Router, RouterModule } from '@angular/router';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTooltipModule } from 'ng-zorro-antd/tooltip';

@Component({
    selector: 'app-stage-definition-list',
    imports: [NzTreeModule, NzIconModule, NzDropDownModule, NzMenuModule, CommonModule, NzCardModule, NzTagModule,
        NzSpinModule, NzEmptyModule, NzDividerModule, NzButtonModule, NzTooltipModule, RouterModule],
    templateUrl: './stage-definition-list.component.html',
    styleUrl: './stage-definition-list.component.css'
})
export class StageDefinitionListComponent implements OnInit {
  treeData: NzTreeNodeOptions[] = [];
  expandedKeys: string[] = [];
  selectedKeys: string[] = [];
  isLoading = true;
  activatedNode: any;

  icons = {
    template: 'safety-certificate',
    defaultStage: 'control',
    submit: 'upload',
    review: 'audit',
    approve: 'check-circle',
    details: 'info-circle'
  };

   readonly router = inject(Router);
   readonly nzContextMenuService = inject(NzContextMenuService);
   readonly _stageDefiService = inject(StageDefinitionService);

  ngOnInit(): void {
    this.loadStageDefinitions();
  }

  loadStageDefinitions(): void {
    this.isLoading = true;
    this._stageDefiService.getStageDefiList().subscribe({
      next: (data: GetStageDefiModel[]) => {
        this.treeData = this.createTreeData(data);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load stage definitions:', err);
        this.isLoading = false;
      }
    });
  }

  private createTreeData(data: GetStageDefiModel[]): NzTreeNodeOptions[] {
    const grouped = data.reduce((acc, stage) => {
      if (!acc[stage.templateId]) acc[stage.templateId] = [];
      acc[stage.templateId].push(stage);
      return acc;
    }, {} as Record<string, GetStageDefiModel[]>);

    return Object.entries(grouped).map(([templateId, stages]) => ({
      title: `Approval Template: ${templateId}`,
      key: `template-${templateId}`,
      expanded: false,
      icon: this.icons.template,
      isTemplate: true,
      children: stages.sort((a, b) => a.sequenceOrder - b.sequenceOrder).map(stage => ({
        title: stage.stageName,
        key: stage.stageDefId,
        isLeaf: true,
        icon: this.getStageIcon(stage),
        stageData: stage,
        routerLink: `/stages/${stage.stageDefId}`
      }))
    }));
  }

  private getStageIcon(stage: GetStageDefiModel): string {
    const name = stage.stageName.toLowerCase();
    if (name.includes('submit')) return this.icons.submit;
    if (name.includes('review')) return this.icons.review;
    if (name.includes('approve')) return this.icons.approve;
    return this.icons.defaultStage;
  }

  onNodeClick(event: NzFormatEmitEvent): void {
    if (!event?.node) return;
    
    this.activatedNode = event.node;
    this.selectedKeys = [event.node.key];

    if (event.node.isLeaf && event.node.origin['routerLink']) {
      this.router.navigate([event.node.origin['routerLink']]);
    } else if (!event.node.isLeaf) {
      event.node.isExpanded = !event.node.isExpanded;
    }
  }

  showStageDetails(stage: GetStageDefiModel): void {
    console.log('Stage details:', stage);
    this.router.navigate(['/stages', stage.stageDefId, 'details']);
  }

  contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent): void {
    this.nzContextMenuService.create($event, menu);
  }
}