import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class UtilitiesService {
    constructor() { }

    UnflatteringForLeftMenu = (arr: any[]): any[] => {
        const map = {};
        const roots: any[] = [];
        for (let i = 0; i < arr.length; i++) {
            const node = arr[i];
            node.children = [];
            map[node.id] = i; // use map to look-up the parents
            if (node.parentId !== null) {
                delete node['children'];
                arr[map[node.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }

        return roots;
    }

    UnflatteringForTree = (arr: any[]): any[] => {
        const roots = [];
        const map = {};
        let node = {
            data: {
                id: '',
                parentId: ''
            },
            expanded: true,
            children: []
        };

        for (let i = 0; i < arr.length; i++) {
            map[arr[i].id] = i;
            arr[i].data = this.GetAllProperties(arr[i]);
            arr[i].children = [];
        }

        for (let i = 0; i < arr.length; i++) {
            node = arr[i];
            if (node.data.parentId !== null && arr[map[node.data.parentId]] !== undefined) {
                arr[map[node.data.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }

        return roots;
    }

    private GetAllProperties = (obj: object): object => {
        const data = {};

        for (const [key, val] of Object.entries(obj)) {
            if (obj.hasOwnProperty(key)) {
                if (typeof val !== 'object') {
                    data[key] = val;
                }
            }
        }

        return data;
    }

    ToFormData(formValue: any) {
        const formData = new FormData();
        for (const key of Object.keys(formValue)) {
            const value = formValue[key];
            if (value) formData.append(key, value);
        }
        return formData;
    }
}
